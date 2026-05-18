using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskySotS.Tweaks.Progression
{
    public class ColossusAccessShrine
    {
        public static GameObject ShrinePrefab;
        public static InspectDef ShrineInspectDef;
        public static InteractableSpawnCard iscColossusAccessShrine;

        public ColossusAccessShrine()
        {
            BuildShrineInspectDef();
            BuildShrinePrefab();
            BuildSpawnCard();
        }

        private void BuildShrineInspectDef()
        {
            ShrineInspectDef = ScriptableObject.CreateInstance<InspectDef>();
            ShrineInspectDef.Info = new RoR2.UI.InspectInfo
            {
                Visual = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/ShrineIcon.png").WaitForCompletion(),
                TitleToken = "SHRINE_COLOSSUS_RISKYSOTS_NAME",
                DescriptionToken = "SHRINE_COLOSSUS_RISKYSOTS_DESCRIPTION",
                FlavorToken = "SHRINE_COLOSSUS_RISKYSOTS_LORE"
            };
        }

        private void BuildShrinePrefab()
        {
            //Already registered to net catalog when you set true
            GameObject prefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/ShrineColossusAccess.prefab").WaitForCompletion().InstantiateClone("RiskySotS_ShrineColossusAccess", true);

            PurchaseInteraction pi = prefab.GetComponent<PurchaseInteraction>();
            pi.costType = CostTypeIndex.None;
            pi.displayNameToken = "SHRINE_COLOSSUS_RISKYSOTS_NAME";
            pi.contextToken = "SHRINE_COLOSSUS_RISKYSOTS_CONTEXT";

            pi.onPurchase = null;
            pi.onDetailedPurchaseServer = null;
            Object.Destroy(prefab.GetComponent<ShrineColossusAccessBehavior>());    //Revive Shrine behavior
            prefab.AddComponent<ColossusAccessShrineBehavior>();

            /*GenericDisplayNameProvider gdn = prefab.GetComponent<GenericDisplayNameProvider>();
            gdn.name = "SHRINE_COLOSSUS_RISKYSOTS_NAME";*/

            GenericInspectInfoProvider gii = prefab.GetComponent<GenericInspectInfoProvider>();
            gii.InspectInfo = ShrineInspectDef;

            ShrinePrefab = prefab;
        }

        private void BuildSpawnCard()
        {
            InteractableSpawnCard isc = ScriptableObject.CreateInstance<InteractableSpawnCard>();
            isc.prefab = ShrinePrefab;
            isc.sendOverNetwork = true;
            isc.hullSize = HullClassification.Golem;
            isc.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            isc.requiredFlags = RoR2.Navigation.NodeFlags.None;
            isc.forbiddenFlags = RoR2.Navigation.NodeFlags.NoShrineSpawn;
            isc.occupyPosition = true;
            isc.directorCreditCost = 0;
            isc.slightlyRandomizeOrientation = false;

            iscColossusAccessShrine = isc;
        }
    }
}
