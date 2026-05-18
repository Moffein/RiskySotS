using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace RiskySotS.Tweaks.Progression
{
    [RequireComponent(typeof(PurchaseInteraction))]
    public class ColossusAccessShrineBehavior : MonoBehaviour
    {
        private PurchaseInteraction purchaseInteraction;

        //No access to Unity Events, so use this hacky workaround.
        private void PurchaseInteractionHack(On.RoR2.PurchaseInteraction.orig_OnInteractionBegin orig, PurchaseInteraction self, Interactor activator)
        {
            //This is solely a hacky workaround for detecting when players lost beads
            bool isCorrectShrine = self == purchaseInteraction && purchaseInteraction != null;

            orig(self, activator);

            if (isCorrectShrine)
            {
                OnPurchaseServer();
            }
        }

        private void Awake()
        {
            On.RoR2.PurchaseInteraction.OnInteractionBegin += PurchaseInteractionHack;
            purchaseInteraction = GetComponent<PurchaseInteraction>();
        }

        private void OnDestroy()
        {
            On.RoR2.PurchaseInteraction.OnInteractionBegin -= PurchaseInteractionHack;
        }

        public void OnPurchaseServer()
        {
            if (!NetworkServer.active) return;
            purchaseInteraction.SetAvailable(false);

            SceneDef currentScene = SceneCatalog.GetSceneDefForCurrentScene();
            if (currentScene)
            {
                if (currentScene.stageOrder == 1)
                {
                    ProgressionRework.RunVariables.hitShrineStage1 = true;
                }
                else
                {
                    ProgressionRework.RunVariables.hitShrineStage1 = true;
                    ProgressionRework.RunVariables.hitShrineStage2 = true;
                }
            }

            Chat.SendBroadcastChat(new Chat.SimpleChatMessage
            {
                baseToken = ProgressionRework.RunVariables.hitShrineStage2 ? "PORTAL_STORM_WILL_OPEN" : "SHRINE_COLOSSUS_RISKYSOTS_ACTIVATION_1"
            });

            if (TeleporterInteraction.instance)
            {
                PortalSpawner[] portals = TeleporterInteraction.instance.gameObject.GetComponents<PortalSpawner>();
                for(int i = 0; i < portals.Length; i++)
                {
                    var portal = portals[i];

                    //Portal functionality was stripped, only orb shows.
                    if (portal.previewChildName == "StormPortalIndicator")
                    {
                        portal.enabled = true;
                        break;
                    }
                }
            }
        }
    }
}
