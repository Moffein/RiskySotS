using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;

namespace RiskySotS.Tweaks.Enemies
{
    public class EarlygameSpawnPools
    {
        public static bool enabled;
        public EarlygameSpawnPools()
        {
            if (!enabled) return;
            ModifyLemurianTemple();

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.RiskyLives.RiskyMod")) return;
            ModifyLakesNight();
            ModifyVillageNight();
        }

        internal DirectorCard BuildDirectorCard(CharacterSpawnCard spawnCard)
        {
            return BuildDirectorCard(spawnCard, 1, 0, DirectorCore.MonsterSpawnDistance.Standard);
        }

        internal DirectorCard BuildDirectorCard(CharacterSpawnCard spawnCard, int weight, int minStages, DirectorCore.MonsterSpawnDistance spawnDistance)
        {
            DirectorCard dc = new DirectorCard
            {
                spawnCard = spawnCard,
                selectionWeight = weight,
                preventOverhead = false,
                minimumStageCompletions = minStages,
                spawnDistance = spawnDistance,
                forbiddenUnlockableDef = null,
                requiredUnlockableDef = null
            };
            return dc;
        }

        private void ModifyLakesNight()
        {
            void ModifyCards(DirectorCardCategorySelection dccs, List<SpawnCard> cardList)
            {
                foreach (var category in dccs.categories)
                {
                    foreach (var card in category.cards)
                    {
                        if (card.minimumStageCompletions < 3 && cardList.Contains(card.spawnCard))
                        {
                            card.minimumStageCompletions = 3;
                        }
                    }
                }
            }

            List<SpawnCard> cardsToModify = new List<SpawnCard>()
            {
                Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/LemurianBruiser/cscLemurianBruiser.asset").WaitForCompletion(),
                Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Parent/cscParent.asset").WaitForCompletion(),
                Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Nullifier/cscNullifier.asset").WaitForCompletion(),
                Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/DLC1/Gup/cscGupBody.asset").WaitForCompletion(),
                Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Grandparent/cscGrandparent.asset").WaitForCompletion(),
                Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/ImpBoss/cscImpBoss.asset").WaitForCompletion(),
            };

            var basePool = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/DLC2/lakesnight/dccsLakesnightMonsters.asset").WaitForCompletion();
            var dlc2Pool = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/DLC2/lakesnight/dccsLakesnightMonsters_DLC1.asset").WaitForCompletion();

            ModifyCards(basePool, cardsToModify);
            ModifyCards(dlc2Pool, cardsToModify);
        }

        private void ModifyVillageNight()
        {
            void ModifyCards(DirectorCardCategorySelection dccs, List<SpawnCard> cardList)
            {
                foreach (var category in dccs.categories)
                {
                    foreach (var card in category.cards)
                    {
                        if (card.minimumStageCompletions < 3 && cardList.Contains(card.spawnCard))
                        {
                            card.minimumStageCompletions = 3;
                        }
                    }
                }
            }

            List<SpawnCard> cardsToModify = new List<SpawnCard>()
            {
                Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/LemurianBruiser/cscLemurianBruiser.asset").WaitForCompletion(),
                Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Parent/cscParent.asset").WaitForCompletion(),
                Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Grandparent/cscGrandparent.asset").WaitForCompletion()
            };

            ModifyCards(Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/DLC2/villagenight/dccsVillageNightMonsters_Additional.asset").WaitForCompletion(), cardsToModify);
        }

        private void ModifyLemurianTemple()
        {
            void ModifyCards(DirectorCardCategorySelection dccs, List<SpawnCard> cardList)
            {
                foreach (var category in dccs.categories)
                {
                    foreach (var card in category.cards)
                    {
                        if (card.minimumStageCompletions < 3 && cardList.Contains(card.spawnCard))
                        {
                            card.minimumStageCompletions = 3;
                        }
                    }
                }
            }

            List<SpawnCard> cardsToModify = new List<SpawnCard>()
            {
                Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/LemurianBruiser/cscLemurianBruiser.asset").WaitForCompletion()
            };

            ModifyCards(Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/DLC2/lemuriantemple/dccsLemurianTempleMonsters.asset").WaitForCompletion(), cardsToModify);
        }
    }
}
