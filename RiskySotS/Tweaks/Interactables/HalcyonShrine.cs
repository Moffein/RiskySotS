using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine;
using RoR2;

namespace RiskySotS.Tweaks.Interactables
{
    public class HalcyonShrine
    {
        public static bool fasterCharging;
        public static bool consistentScaling;

        public HalcyonShrine()
        {
            if (fasterCharging)
            {
                ChargeFaster();
            }

            if (consistentScaling)
            {
                ScaleConsistently();
            }
        }

        private void ChargeFaster()
        {
            //Yoinked from https://github.com/viliger2/RoR2_SmallerMods/blob/main/SpeedUpHalcyoniteShrine/SpeedUpHalcyoniteShrine.cs
            var shrineHalcyonite = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_DLC2.ShrineHalcyonite_prefab).WaitForCompletion();
            var interactable = shrineHalcyonite.GetComponent<HalcyoniteShrineInteractable>();
            interactable.tickRate = 10;
        }

        private void ScaleConsistently()
        {
            On.RoR2.CombatDirector.HalcyoniteShrineActivation += CombatDirector_HalcyoniteShrineActivation;
        }

        private void CombatDirector_HalcyoniteShrineActivation(On.RoR2.CombatDirector.orig_HalcyoniteShrineActivation orig, CombatDirector self, float monsterCredit, DirectorCard chosenDirectorCard, int difficultyLevel, Transform shrineTransform)
        {
            int diffOverride = difficultyLevel;
            if (self.gameObject && Run.instance)
            {
                var halc = self.gameObject.GetComponent<HalcyoniteShrineInteractable>();
                if (halc)
                {
                    float diff = Stage.instance ? Stage.instance.entryDifficultyCoefficient : Run.instance.difficultyCoefficient;
                    int minChargeCost = Run.instance.GetDifficultyScaledCost(halc.midGoldCost, diff);
                    int maxChargeCost = Run.instance.GetDifficultyScaledCost(halc.maxGoldCost, diff);

                    float chargePercent = Mathf.InverseLerp(minChargeCost, maxChargeCost, halc.goldDrained);

                    //Stage 1 Halc is 0/1/3
                    //Stage 3 is typically 15ish, 10ish is what a normal Gilded Halcyonite should be
                    int minDiff = 4;
                    int maxDiff = 10;

                    int calculatedDiff = Mathf.FloorToInt(minDiff + (maxDiff - minDiff) * chargePercent);
                    diffOverride = calculatedDiff;
                }
            }

            orig(self, monsterCredit, chosenDirectorCard, diffOverride, shrineTransform);
        }
    }
}
