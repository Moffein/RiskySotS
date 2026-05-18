using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RiskySotS
{
    internal static class CommonHooks
    {
        internal static void DisableRemoveBuffGeneric(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(x => x.MatchCallvirt(typeof(CharacterBody), "RemoveBuff")))
            {
                c.EmitDelegate<Func<BuffIndex, BuffIndex>>(orig => BuffIndex.None);
            }
            else
            {
                Debug.LogError("RiskySotS: DisableRemoveBuffGeneric IL Hook failed");
            }
        }

        internal static void DisableAddBuffGeneric(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(x => x.MatchCallvirt(typeof(CharacterBody), "AddBuff")))
            {
                c.EmitDelegate<Func<BuffIndex, BuffIndex>>(orig => BuffIndex.None);
            }
            else
            {
                Debug.LogError("RiskySotS: DisableAddBuffGeneric IL Hook failed");
            }
        }
    }
}
