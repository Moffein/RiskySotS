using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine;
using RoR2;

namespace RiskySotS.Tweaks.Enemies
{
    public class Scorchling
    {
        public static bool removeIFrames;
        public static bool nerfDamage;

        public Scorchling()
        {
            NerfDamage();
            RemoveIFrames();
        }

        private void NerfDamage()
        {
            if (!nerfDamage) return;
            SneedUtils.SetAddressableEntityStateField("RoR2/DLC2/Scorchling/EntityStates.Scorchling.LavaBomb.asset", "mortarDamageCoefficient", "0.8");
        }

        private void RemoveIFrames()
        {
            if (!removeIFrames) return;
            IL.ScorchlingController.Burrow += CommonHooks.DisableAddBuffGeneric;
            IL.ScorchlingController.Breach += CommonHooks.DisableRemoveBuffGeneric;
        }
    }
}
