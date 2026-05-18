using UnityEngine.AddressableAssets;
using UnityEngine;
using RoR2.Projectile;
using R2API;

namespace RiskySotS.Tweaks.Enemies
{
    public class Child
    {
        public static bool removeIFrames;
        public static bool nerfDamage;

        public Child()
        {
            NerfDamage();
            RemoveIFrames();
        }

        private void NerfDamage()
        {
            if (!nerfDamage) return;
            SneedUtils.SetAddressableEntityStateField("RoR2/DLC2/Child/EntityStates.ChildMonster.FireTrackingSparkBall.asset", "bombDamageCoefficient", "4");

            GameObject projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/Child/ChildTrackingSparkBall.prefab").WaitForCompletion().InstantiateClone("RiskyModChildTrackingSparkball", true);
            var pie = projectilePrefab.GetComponent<ProjectileExplosion>();
            pie.falloffModel = RoR2.BlastAttack.FalloffModel.HalfLinear;

            R2API.ContentAddition.AddProjectile(projectilePrefab);
            SneedUtils.SetAddressableEntityStateField("RoR2/DLC2/Child/EntityStates.ChildMonster.FireTrackingSparkBall.asset", "projectilePrefab", projectilePrefab);
        }

        private void RemoveIFrames()
        {
            if (!removeIFrames) return;
            On.RoR2.ChildMonsterController.RegisterTeleport += ChildMonsterController_RegisterTeleport;
        }

        private void ChildMonsterController_RegisterTeleport(On.RoR2.ChildMonsterController.orig_RegisterTeleport orig, RoR2.ChildMonsterController self, bool addInvincibility)
        {
            orig(self, false);
        }
    }
}
