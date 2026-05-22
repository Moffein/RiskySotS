using RoR2.ExpansionManagement;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Runtime.CompilerServices;
namespace RiskySotS.Tweaks.Progression
{
    public class ProgressionRework
    {
        public static bool enabled;
        public static bool allowVariantsPreLoop;
        public static float goldShrineChance = 50f;

        public static ExpansionDef dlc2Expansion = Addressables.LoadAssetAsync<ExpansionDef>("RoR2/DLC2/Common/DLC2.asset").WaitForCompletion();
        private SpawnCard iscShrineHalcyonite = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/DLC2/iscShrineHalcyonite.asset").WaitForCompletion();
        private SpawnCard iscShrineHalcyoniteTier1 = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/DLC2/iscShrineHalcyoniteTier1.asset").WaitForCompletion();
        private SpawnCard iscShrineGold = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/ShrineGoldshoresAccess/iscShrineGoldshoresAccess.asset").WaitForCompletion();

        public class RunVariables
        {
            public static bool hitShrineStage1 = false;
            public static bool hitShrineStage2 = false;
            public static bool enteredMeridian = false;

            public static bool spawnedGoldShrineThisLoop = false;
            public static bool spawnedGoldShrineThisStage = false;

            public static bool hitShrineThisStage = false;
        }

        public ProgressionRework()
        {
            if (!enabled) return;
            new ColossusAccessShrine();
            AddStagesToMainline();
            ReworkGreenPortalLogic();
            ReworkHalcyonShrineProgression();

            RoR2.Run.onRunStartGlobal += Run_onRunStartGlobal;
            SceneDirector.onPrePopulateSceneServer += SceneDirector_onPrePopulateSceneServer;
            SceneDirector.onGenerateInteractableCardSelection += FilterGoldShrineIfForceSpawned;
            On.RoR2.GoldshoresMissionController.OnEnable += GoldshoresMissionController_OnEnable;
            On.RoR2.MeridianEventTriggerInteraction.OnEnable += MeridianEventTriggerInteraction_OnEnable;

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.KingEnderBrine.ProperSave"))
            {
                HandleSave();
            }

            new HabitatAccessNodes();
        }

        private void FilterGoldShrineIfForceSpawned(SceneDirector director, DirectorCardCategorySelection dccs)
        {
            dccs.RemoveCardsThatFailFilter(card => card.spawnCard != iscShrineGold || !RunVariables.spawnedGoldShrineThisStage);
        }

        private void MeridianEventTriggerInteraction_OnEnable(On.RoR2.MeridianEventTriggerInteraction.orig_OnEnable orig, MeridianEventTriggerInteraction self)
        {
            orig(self);
            RunVariables.enteredMeridian = true;
        }

        private void GoldshoresMissionController_OnEnable(On.RoR2.GoldshoresMissionController.orig_OnEnable orig, GoldshoresMissionController self)
        {
            orig(self);
            RunVariables.hitShrineStage1 = true;
            RunVariables.hitShrineStage2 = true;
        }

        private void SceneDirector_onPrePopulateSceneServer(SceneDirector director)
        {
            RunVariables.hitShrineThisStage = false;
            RunVariables.spawnedGoldShrineThisStage = false;
            if (Run.instance && !RunVariables.enteredMeridian && Run.instance.IsExpansionEnabled(dlc2Expansion))
            {
                bool shouldSpawnAccess = false;
                bool shouldSpawnHalc = false;
                bool shouldSpawnGoldShrine = false;

                SceneDef currentScene = SceneCatalog.GetSceneDefForCurrentScene();
                if (currentScene)
                {
                    if (currentScene.sceneType == SceneType.Stage && !currentScene.blockOrbitalSkills)
                    {
                        if (currentScene.stageOrder == 1)
                        {
                            RunVariables.spawnedGoldShrineThisLoop = false;
                            shouldSpawnAccess = true;
                        }
                        else if (currentScene.stageOrder == 2 && RunVariables.hitShrineStage1)
                        {
                            shouldSpawnAccess = true;
                            if (!RunVariables.spawnedGoldShrineThisLoop) shouldSpawnGoldShrine = Util.CheckRoll(goldShrineChance);
                        }
                        else if (currentScene.stageOrder == 3 && RunVariables.hitShrineStage2)
                        {
                            shouldSpawnHalc = true;
                            if (!RunVariables.spawnedGoldShrineThisLoop) shouldSpawnGoldShrine = Util.CheckRoll(goldShrineChance);
                        }
                        else
                        {
                            RunVariables.hitShrineStage1 = false;
                            RunVariables.hitShrineStage2 = false;
                        }
                    }
                }

                //Spawn things if appropriate
                if (shouldSpawnGoldShrine)
                {
                    RunVariables.spawnedGoldShrineThisStage = true;
                    Debug.Log("RiskySotS: Spawning random chance Altar of Gold.");
                    DirectorPlacementRule placementRule = new DirectorPlacementRule
                    {
                        placementMode = DirectorPlacementRule.PlacementMode.Random
                    };
                    GameObject obj = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(iscShrineGold, placementRule, director.rng));
                    if (obj)
                    {
                        director.interactableCredit--;
                        RunVariables.spawnedGoldShrineThisLoop = true;
                    }
                }

                if (shouldSpawnHalc)
                {
                    Debug.Log("RiskySotS: Spawning Halcyonite Shrine.");
                    //Scale credits because rewards scale with players
                    float creditMult = 1f + (0.5f * (Run.instance.participatingPlayerCount - 1f));

                    DirectorPlacementRule placementRule = new DirectorPlacementRule
                    {
                        placementMode = DirectorPlacementRule.PlacementMode.Random
                    };
                    GameObject obj = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(iscShrineHalcyonite, placementRule, director.rng));

                    //Vanilla Cost: 50
                    //Large Chest: 30
                    //Seems about right if not slightly low, but I'll tolerate it.
                    if (obj)
                    {
                        director.interactableCredit -= Mathf.FloorToInt(50 * creditMult);
                    }
                }

                if (shouldSpawnAccess)
                {
                    Debug.Log("RiskySotS: Spawning Shrine of the Colossus.");
                    DirectorPlacementRule placementRule = new DirectorPlacementRule
                    {
                        placementMode = DirectorPlacementRule.PlacementMode.Random
                    };
                    DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(ColossusAccessShrine.iscColossusAccessShrine, placementRule, director.rng));
                }
            }
        }

        private void Run_onRunStartGlobal(Run obj)
        {
            RunVariables.hitShrineStage1 = false;
            RunVariables.hitShrineStage2 = false;
            RunVariables.enteredMeridian = false;
            RunVariables.spawnedGoldShrineThisStage = false;
            RunVariables.spawnedGoldShrineThisLoop = false;
        }

        private void AddStagesToMainline()
        {
            SceneDef sots1 = Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC2/village/village.asset").WaitForCompletion();
            SceneDef sots2 = Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC2/lemuriantemple/lemuriantemple.asset").WaitForCompletion();
            SceneDef sots3 = Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC2/habitat/habitat.asset").WaitForCompletion();

            SceneDef devo = Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC2/lakes/lakes.asset").WaitForCompletion();
            SceneDef devoLoop = Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC2/lakesnight/lakesnight.asset").WaitForCompletion();
            SceneDef sots1Loop = Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC2/villagenight/villagenight.asset").WaitForCompletion();
            SceneDef sots3Loop = Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC2/habitatfall/habitatfall.asset").WaitForCompletion();

            SceneCollection sgStage1 = Addressables.LoadAssetAsync<SceneCollection>("RoR2/Base/SceneGroups/sgStage1.asset").WaitForCompletion();
            SceneCollection sgStage2 = Addressables.LoadAssetAsync<SceneCollection>("RoR2/Base/SceneGroups/sgStage2.asset").WaitForCompletion();
            SceneCollection sgStage3 = Addressables.LoadAssetAsync<SceneCollection>("RoR2/Base/SceneGroups/sgStage3.asset").WaitForCompletion();

            SceneCollection sgStage1Loop = Addressables.LoadAssetAsync<SceneCollection>("RoR2/Base/SceneGroups/loopSgStage1.asset").WaitForCompletion();
            SceneCollection sgStage2Loop = Addressables.LoadAssetAsync<SceneCollection>("RoR2/Base/SceneGroups/loopSgStage2.asset").WaitForCompletion();
            SceneCollection sgStage3Loop = Addressables.LoadAssetAsync<SceneCollection>("RoR2/Base/SceneGroups/loopSgStage3.asset").WaitForCompletion();

            //LemurianTemple
            AddSceneToCollection(sots2, sgStage2);
            AddSceneToCollection(sots2, sgStage2Loop);

            if (allowVariantsPreLoop)
            {
                if (!RiskySotSPlugin.loopVariantPluginLoaded)
                {
                    Debug.LogWarning("RiskySotS: Progression Rework - Detected LoopVariantConfig. Stage 1 variants will NOT be added to the pool. Check the LoopVariantConfig config file!");
                    //Pre-Loop: Adjust Lakes weight and add LakeNight
                    ModifySceneWeightInCollection(devo, sgStage1, 0.5f);
                    AddSceneToCollection(devoLoop, sgStage1, 0.5f);

                    //Post-Loop: Adjust LakesNight weight and re-add Lakes
                    ModifySceneWeightInCollection(devoLoop, sgStage1Loop, 0.5f);
                    AddSceneToCollection(devo, sgStage1Loop, 0.5f);

                    //Pre-Loop: Adjust Village weight and add VillageNight
                    ModifySceneWeightInCollection(sots1, sgStage1, 0.5f);
                    AddSceneToCollection(sots1Loop, sgStage1, 0.5f);

                    //Post-Loop: Adjust VillageNight weight and add Village
                    AddSceneToCollection(sots1, sgStage1Loop, 0.5f);
                    ModifySceneWeightInCollection(sots1Loop, sgStage1Loop, 0.5f);
                }

                //Habitat + HabitatFall, pre-loop and post-loop
                AddSceneToCollection(sots3, sgStage3, 0.5f);
                AddSceneToCollection(sots3Loop, sgStage3, 0.5f);

                AddSceneToCollection(sots3, sgStage3Loop, 0.5f);
                AddSceneToCollection(sots3Loop, sgStage3Loop, 0.5f);
            }
            else
            {
                //Habitat + HabitatFall
                AddSceneToCollection(sots3, sgStage3);
                AddSceneToCollection(sots3Loop, sgStage3Loop);
            }
        }

        private void ModifySceneWeightInCollection(SceneDef scene, SceneCollection collection, float newWeight)
        {
            var list = collection._sceneEntries.ToList();
            for(int i = 0; i < list.Count; i++)
            {
                var entry = list[i];
                if (entry.sceneDef == scene)
                {
                    entry.weight = newWeight;
                    break;
                }
            }
            collection._sceneEntries = list.ToArray();
        }

        private void AddSceneToCollection(SceneDef scene, SceneCollection collection, float weight = 1f)
        {
            //Check if scene is already in collection
            foreach (var entry in collection.sceneEntries)
            {
                if (entry.sceneDef == scene) return;
            }

            var list = collection._sceneEntries.ToList();
            list.Add(new SceneCollection.SceneEntry()
            {
                sceneDef = scene,
                weight = weight
            });

            collection._sceneEntries = list.ToArray();
        }

        private void ReworkHalcyonShrineProgression()
        {
            //Obliterate Halcyon Shrines from regular drop pool
            On.RoR2.DCCSBlender.GetBlendedDCCS += DCCSBlender_GetBlendedDCCS;
        }


        private DirectorCardCategorySelection DCCSBlender_GetBlendedDCCS(On.RoR2.DCCSBlender.orig_GetBlendedDCCS orig, DccsPool.Category dccsPoolCategory, ref Xoroshiro128Plus rng, ClassicStageInfo stageInfo, int contentSourceMixLimit, System.Collections.Generic.List<RoR2.ExpansionManagement.ExpansionDef> acceptableExpansionList)
        {
            var ret = orig(dccsPoolCategory, ref rng, stageInfo, contentSourceMixLimit, acceptableExpansionList);
            if (ret != null && ret.categories != null)
            {
                for (int i = 0; i < ret.categories.Length; i++)
                {
                    if (ret.categories[i].cards != null)
                    {
                        ret.categories[i].cards = ret.categories[i].cards.Where(card => !(card != null && (card.spawnCard == iscShrineHalcyonite || card.spawnCard == iscShrineHalcyoniteTier1))).ToArray();
                        ret.categories = ret.categories.Where(cat => cat.cards != null && cat.cards.Length > 0).ToArray();
                    }
                }
            }
            return ret;
        }

        private static GameObject miniGeode = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/MiniGeodeBody.prefab").WaitForCompletion();
        private void ReworkGreenPortalLogic()
        {
            //Green Portal only ever points towards Meridian
            GameObject greenPortal = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/PortalColossus.prefab").WaitForCompletion();
            greenPortal.GetComponent<SceneExitController>().isAlternatePath = false;

            //Only spawn Green Portal in GoldShores if it leads to Meridian
            On.EntityStates.Missions.Goldshores.Exit.IsValidStormTier += Exit_IsValidStormTier;

            //Remove geodes from Bazaar
            On.RoR2.Networking.GenericSceneSpawnPoint.Start += GenericSceneSpawnPoint_Start;

            InteractableSpawnCard colossusPortal = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/DLC2/iscColossusPortal.asset").WaitForCompletion();

            //Rework Green Orbs on TP
            ReworkGreenOrbs(colossusPortal);

            //Force Haclyonite Shrine to only spawn Green Orbs
            ReworkHalcyoniteShrinePortals(colossusPortal);

            On.RoR2.PortalSpawner.AttemptSpawnPortalServer += PortalSpawner_AttemptSpawnPortalServer;
        }

        private bool PortalSpawner_AttemptSpawnPortalServer(On.RoR2.PortalSpawner.orig_AttemptSpawnPortalServer orig, PortalSpawner self)
        {
            if (self.portalSpawnCard == null && self.spawnMessageToken == "SHRINE_COLOSSUS_RISKYSOTS_TP_FINISH" && RunVariables.hitShrineThisStage)
            {

                Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                {
                    baseToken = self.spawnMessageToken
                });
                return false;
            }
            return orig(self);
        }

        private void ReworkHalcyoniteShrinePortals(InteractableSpawnCard colossusPortal)
        {
            GameObject shrineHalcyonite = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_DLC2.ShrineHalcyonite_prefab).WaitForCompletion();
            PortalSpawner[] portals = shrineHalcyonite.GetComponents<PortalSpawner>();
            foreach (PortalSpawner ps in portals)
            {
                if (ps.portalSpawnCard == colossusPortal)
                {
                    ps.invalidStages = new string[0];
                    ps.validStageTiers = new int[0];
                }
                else
                {
                    ps.enabled = false;
                }
            }
        }

        private void ReworkGreenOrbs(InteractableSpawnCard colossusPortal)
        {
            GameObject teleporter = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Teleporters/Teleporter1.prefab").WaitForCompletion();
            PortalSpawner[] portals = teleporter.GetComponents<PortalSpawner>();
            foreach (var portal in portals)
            {
                //Remove Portal functionality, but the green orb will show when ColossusAccess calls it.
                if (portal.portalSpawnCard == colossusPortal)
                {
                    portal.enabled = false;

                    portal.spawnPreviewMessageToken = "";
                    portal.spawnMessageToken = "SHRINE_COLOSSUS_RISKYSOTS_TP_FINISH";

                    portal.validStages = new string[0];

                    portal.portalSpawnCard = null;
                    break;
                }
            }
        }

        private void GenericSceneSpawnPoint_Start(On.RoR2.Networking.GenericSceneSpawnPoint.orig_Start orig, RoR2.Networking.GenericSceneSpawnPoint self)
        {
            if (self.networkedObjectPrefab == miniGeode)
            {
                SceneDef currentScene = SceneCatalog.GetSceneDefForCurrentScene();
                if (currentScene && currentScene.baseSceneName == "bazaar")
                {
                    Debug.LogWarning("RiskySotS: Intercepted MiniGeodeBody spawn in Bazaar. Spawn will not proceed.");
                    self.gameObject.SetActive(false);
                }
                return;
            }

            orig(self);
        }

        private bool Exit_IsValidStormTier(On.EntityStates.Missions.Goldshores.Exit.orig_IsValidStormTier orig, EntityStates.Missions.Goldshores.Exit self)
        {
            //Disable green portal unless it leads to Meridian
            return Run.instance && Run.instance.IsExpansionEnabled(dlc2Expansion) && Run.instance.nextStageScene.stageOrder == 4 && !RunVariables.enteredMeridian;
        }



        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void HandleSave()
        {
            ProperSave.SaveFile.OnGatherSaveData += Save;
            ProperSave.Loading.OnLoadingEnded += Load;
        }

        public static void Save(Dictionary<string, object> dict)
        {
            dict.Add("riskySots.hitShrineStage1", RunVariables.hitShrineStage1);
            dict.Add("riskySots.hitShrineStage2", RunVariables.hitShrineStage2);
            dict.Add("riskySots.enteredMeridian", RunVariables.enteredMeridian);
            dict.Add("riskySots.spawnedGoldShrineThisLoop", RunVariables.spawnedGoldShrineThisLoop);
            dict.Add("riskySots.spawnedGoldShrineThisStage", RunVariables.spawnedGoldShrineThisStage);
        }

        public void Load(ProperSave.SaveFile save)
        {
            RunVariables.hitShrineStage1 = save.GetModdedData<bool>("riskySots.hitShrineStage1");
            RunVariables.hitShrineStage2 = save.GetModdedData<bool>("riskySots.hitShrineStage2");
            RunVariables.enteredMeridian = save.GetModdedData<bool>("riskySots.enteredMeridian");
            RunVariables.spawnedGoldShrineThisLoop = save.GetModdedData<bool>("riskySots.spawnedGoldShrineThisLoop");
            RunVariables.spawnedGoldShrineThisStage = save.GetModdedData<bool>("riskySots.spawnedGoldShrineThisStage");
        }
    }
}
