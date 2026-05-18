using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.AddressableAssets;

namespace RiskySotS.Tweaks.Interactables
{
    public class RemoveReviveShrine
    {
        public static bool enabled;
        private SpawnCard shrineCard = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/DLC2/iscShrineColossusAccess.asset").WaitForCompletion();

        public RemoveReviveShrine()
        {
            if (!enabled) return;
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
                        ret.categories[i].cards = ret.categories[i].cards.Where(card => !(card != null && card.spawnCard == shrineCard)).ToArray();
                        ret.categories = ret.categories.Where(cat => cat.cards != null && cat.cards.Length > 0).ToArray();
                    }
                }
            }
            return ret;
        }
    }
}
