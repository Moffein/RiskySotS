using UnityEngine;
using RoR2;
using EntityStates;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using RoR2.ExpansionManagement;
using EntityStates.Missions.AccessCodes.Node;
using System;
using R2API;

namespace RiskySotS.Tweaks.Progression
{
    public class HabitatAccessNodes
    {
        private static ExpansionDef dlc3Expansion = Addressables.LoadAssetAsync<ExpansionDef>("RoR2/DLC3/DLC3.asset").WaitForCompletion();
        public static GameObject habitatAccessNode;

        public struct AccessCodePosition
        {
            public Vector3 position;
            public Vector3 rotation;
        }

        public static AccessCodePosition[] habitatAccessCodePositions = new AccessCodePosition[]
        {
            new AccessCodePosition
            {
                position = new Vector3(-8.4f, 36f, -91f),
                rotation = new Vector3(0f, 0f, 57f)
            },
            new AccessCodePosition
            {
                position = new Vector3(-56.7f, 31f, 2f),
                rotation = new Vector3(-90f, 0f, -30f)
            },
            new AccessCodePosition
            {
                position = new Vector3(12.84f, -8.14f, 131.2f),
                rotation = new Vector3(25.06f, -26.9f, -32.2f)
            },
            new AccessCodePosition
            {
                position = new Vector3(-20f, 25f, -12.5f),
                rotation = Vector3.zero
            }
        };

        private static bool IsHabitat()
        {
            SceneDef currentScene = SceneCatalog.GetSceneDefForCurrentScene();
            return currentScene && (currentScene.baseSceneName == "habitat" || currentScene.baseSceneName == "habitatfall");
        }

        //Massive hacks to skip the AccessCodeNode mission controller
        public HabitatAccessNodes()
        {
            BuildPrefab();
            SceneDirector.onPrePopulateSceneServer += SceneDirector_onPrePopulateSceneServer;
            On.EntityStates.Missions.AccessCodes.Node.AccessNodesBaseState.OnEnter += AccessNodesBaseState_OnEnter;
            On.EntityStates.Missions.AccessCodes.Node.NodeAccessed.OnEnter += NodeAccessed_OnEnter;
            On.EntityStates.Missions.AccessCodes.Node.NodeActive.OnEnter += NodeActive_OnEnter;
        }

        private void BuildPrefab()
        {
            //This auto registers it to catalog
            GameObject accessNode = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC3/AccessCodesNode/Access Codes Node.prefab").WaitForCompletion().InstantiateClone("RiskySotS_HabitatAccessNode", true);
            accessNode.AddComponent<TurnOffNodeOnTeleporterStart>();

            var portalSpawnerCanyon = accessNode.AddComponent<PortalSpawner>();
            portalSpawnerCanyon.portalSpawnCard = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/DLC3/iscHardwareProgPortal.asset").WaitForCompletion();
            portalSpawnerCanyon.minSpawnDistance = 10f;
            portalSpawnerCanyon.maxSpawnDistance = 30f;
            portalSpawnerCanyon.spawnPreviewMessageToken = "PORTAL_SOLUSSHOP_WILL_OPEN";
            portalSpawnerCanyon.spawnMessageToken = "PORTAL_SOLUSSHOP_OPEN";
            portalSpawnerCanyon.previewChildName = "CEPortalIndicator";
            portalSpawnerCanyon.registerSelfToPortalRemotelyOnActivation = true;

            var portalSpawnerCompex = accessNode.AddComponent<PortalSpawner>();
            portalSpawnerCompex.portalSpawnCard = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/DLC3/iscSolusShopPortal.asset").WaitForCompletion();
            portalSpawnerCompex.minSpawnDistance = 10f;
            portalSpawnerCompex.maxSpawnDistance = 30f;
            portalSpawnerCompex.spawnPreviewMessageToken = "PORTAL_SOLUSSHOP_WILL_OPEN";
            portalSpawnerCompex.spawnMessageToken = "PORTAL_SOLUSSHOP_OPEN";
            portalSpawnerCompex.previewChildName = "CEPortalIndicator";
            portalSpawnerCompex.registerSelfToPortalRemotelyOnActivation = true;

            var portalSpawnerHaunt = accessNode.AddComponent<PortalSpawner>();
            portalSpawnerHaunt.portalSpawnCard = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/DLC3/iscHardwareProgPortal.asset").WaitForCompletion();
            portalSpawnerHaunt.minSpawnDistance = 10f;
            portalSpawnerHaunt.maxSpawnDistance = 30f;
            portalSpawnerHaunt.spawnPreviewMessageToken = "PORTAL_SOLUSSHOP_WILL_OPEN";
            portalSpawnerHaunt.spawnMessageToken = "PORTAL_SOLUSSHOP_OPEN";
            portalSpawnerHaunt.previewChildName = "CEPortalIndicator";

            var solusFight = accessNode.AddComponent<SolusFight>();
            solusFight.requiredExpansion = dlc3Expansion;
            solusFight.conduitCanyonPortalSpawner = portalSpawnerCanyon;
            solusFight.computationalExchangePortalSpawner = portalSpawnerCompex;
            solusFight.solutionalHauntPortalSpawner = portalSpawnerHaunt;

            solusFight.ForcedBossFight = new DirectorCard
            {
                selectionWeight = 1,
                spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/DLC3/SolusAmalgamator/cscSolusAmalgamator.asset").WaitForCompletion(),
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            
            habitatAccessNode = accessNode;
        }

        private void NodeActive_OnEnter(On.EntityStates.Missions.AccessCodes.Node.NodeActive.orig_OnEnter orig, NodeActive self)
        {
            if (IsHabitat())
            {
                if (!self.childLocator)
                {
                    self.childLocator = self.GetComponent<ChildLocator>();
                }
                self.SwapMaterials(AccessNodesBaseState.NodeStates.on);
                GameObject effectPrefab = AccessNodesBaseState.completedEffectPrefab;
                if (effectPrefab)
                {
                    Transform transform = self.childLocator.FindChild(AccessNodesBaseState.effectAttachString);
                    EffectData effectData = new EffectData
                    {
                        origin = transform.position,
                        rotation = transform.rotation
                    };
                    effectData.SetChildLocatorTransformReference(transform.gameObject, 0);
                    EffectManager.SpawnEffect(effectPrefab, effectData, true);
                }
                HabitatBaseOnEnter(self);

                if (NetworkServer.active)
                {
                    SolusFight sf = self.GetComponent<SolusFight>();
                    if (sf)
                    {
                        sf.TriggerServer();
                    }
                }
            }
            else
            {
                orig(self);
            }
        }

        private static void HabitatBaseOnEnter(AccessNodesBaseState self)
        {
            self.interactionComponent = self.GetComponent<ProxyInteraction>();
            self.interactionComponent.getContextString = new Func<ProxyInteraction, Interactor, string>(self.GetContextStringInternal);
            self.interactionComponent.getInteractability = new Func<ProxyInteraction, Interactor, Interactability>(self.GetInteractabilityInternal);
            self.interactionComponent.onInteractionBegin = new Action<ProxyInteraction, Interactor>(self.OnInteractionBeginInternal);
            self.interactionComponent.shouldShowOnScanner = new Func<ProxyInteraction, bool>(self.ShouldShowOnScannerInternal);
            self.interactionComponent.shouldIgnoreSpherecastForInteractability = new Func<ProxyInteraction, Interactor, bool>(self.ShouldIgnoreSpherecastForInteractabilityInternal);
            self.childLocator = self.GetComponent<ChildLocator>();
            self.Id = 0;
            Transform transform = self.childLocator.FindChild("OFF");
            if (transform)
            {
                self._nodeMeshAnimator = transform.GetComponent<Animator>();
            }
        }

        private void NodeAccessed_OnEnter(On.EntityStates.Missions.AccessCodes.Node.NodeAccessed.orig_OnEnter orig, NodeAccessed self)
        {
            if (IsHabitat())
            {
                HabitatBaseOnEnter(self);
                if (self.isAuthority)
                {
                    self.outer.SetNextState(new NodeActive()
                    {
                        Id = 0
                    });
                }
            }
            else
            {
                orig(self);
            }
        }

        private void AccessNodesBaseState_OnEnter(On.EntityStates.Missions.AccessCodes.Node.AccessNodesBaseState.orig_OnEnter orig, AccessNodesBaseState self)
        {
            if (IsHabitat())
            {
                HabitatBaseOnEnter(self);
            }
            else
            {
                orig(self);
            }
        }

        private void SceneDirector_onPrePopulateSceneServer(SceneDirector director)
        {
            SceneDef currentScene = SceneCatalog.GetSceneDefForCurrentScene();
            //Spawn habitat access nodes
            if (Run.instance && Run.instance.IsExpansionEnabled(dlc3Expansion) && !Run.instance.GetEventFlag("HardwarePortalTaken") && currentScene)
            {
                if (IsHabitat())
                {
                    int selectedPosition = UnityEngine.Random.RandomRangeInt(0, habitatAccessCodePositions.Length);
                    var info = habitatAccessCodePositions[selectedPosition];

                    var instance = GameObject.Instantiate(habitatAccessNode);
                    if (instance)
                    {
                        EntityStateMachine esm = instance.GetComponent<EntityStateMachine>();
                        if (esm)
                        {
                            esm.initialStateType = new SerializableEntityStateType(typeof(NodesOnAndReady));
                            esm.SetNextState(new NodesOnAndReady
                            {
                                Id = 0
                            });
                            Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                            {
                                baseToken = "ACCESSCODES_SPAWNED"
                            });
                        }

                        NetworkServer.Spawn(instance);
                        instance.transform.position = info.position;
                        instance.transform.rotation = Quaternion.Euler(info.rotation.x, info.rotation.y, info.rotation.z);

                        Debug.Log("RiskySotS: Spawning Access Node on " + currentScene.baseSceneName);
                    }
                    else
                    {
                        Debug.LogError("RiskySotS: CRITICAL FAILURE - Access Node failed to spawn on " + currentScene.baseSceneName +", report this error!");
                    }
                }
            }
        }
    }

    [RequireComponent(typeof(NetworkIdentity))]
    public class TurnOffNodeOnTeleporterStart : MonoBehaviour
    {
        private NetworkIdentity networkIdentity;

        private void Awake()
        {
            networkIdentity = GetComponent<NetworkIdentity>();
        }

        private void FixedUpdate()
        {
            if (TeleporterInteraction.instance && TeleporterInteraction.instance.isCharging)
            {
                if (networkIdentity && Util.HasEffectiveAuthority(networkIdentity))
                {
                    EntityStateMachine esm = GetComponent<EntityStateMachine>();
                    if (esm)
                    {
                        esm.SetNextState(new Off()
                        {
                            Id = 0
                        });
                    }
                }
                Destroy(this);
                return;
            }
        }
    }
}
