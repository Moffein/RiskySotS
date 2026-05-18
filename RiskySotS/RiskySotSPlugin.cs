using BepInEx;
using BepInEx.Configuration;
using R2API.Utils;
using RiskySotS.Tweaks.Enemies;
using RiskySotS.Tweaks.Interactables;
using RiskySotS.Tweaks.Progression;
using System.Security.Permissions;
using System.Security;


[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
namespace RiskySotS
{
    [BepInDependency("Wolfo.LoopVariantConfig", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.RiskyTweaks", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.RiskyLives.RiskyMod", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInDependency(R2API.PrefabAPI.PluginGUID)]
    [BepInDependency(R2API.ContentManagement.R2APIContentManager.PluginGUID)]
    [BepInPlugin("com.RiskyLives.RiskySotS", "RiskySotS", "1.0.0")]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class RiskySotSPlugin : BaseUnityPlugin
    {
        internal static bool loopVariantPluginLoaded;

        public void Awake()
        {
            ReadConfig();
            ModCompat();
            new LanguageTokens();

            //Enemies
            new EarlygameSpawnPools();
            new Halcyonite();
            new Child();
            new Scorchling();

            //Progression
            new ProgressionRework();

            //Interactables
            new NerfInteractableCredits();
            new RemoveReviveShrine();
            new HalcyonShrine();
        }

        private void ReadConfig()
        {
            ProgressionRework.enabled = base.Config.Bind<bool>(new ConfigDefinition("Progression", "Progression Rework"), true, new ConfigDescription("SotS stages are mixed into the normal map pool, and Meridian access is changed.")).Value;
            ProgressionRework.allowVariantsPreLoop = base.Config.Bind<bool>(new ConfigDefinition("Progression", "Progression Rework - Allow Variants Pre-Loop"), true, new ConfigDescription("SotS stage variants can show up on the first loop.")).Value;
            ProgressionRework.goldShrineChance = base.Config.Bind<float>(new ConfigDefinition("Progression", "Progression Rework - Altar of Gold Chance"), 50f, new ConfigDescription("Chance for Altar of Gold to spawn on the next stage after hitting a Shrine of the Colossus.")).Value;

            HalcyonShrine.fasterCharging = base.Config.Bind<bool>(new ConfigDefinition("Interactables", "Halcyon Shrine - Faster Charging"), true, new ConfigDescription("Halcyon Shrine charges faster.")).Value;
            HalcyonShrine.consistentScaling = base.Config.Bind<bool>(new ConfigDefinition("Interactables", "Halcyon Shrine - Consistent Scaling"), true, new ConfigDescription("Halcyon Shrine boss scaling remains consistent throughout the run.")).Value;

            RemoveReviveShrine.enabled = base.Config.Bind<bool>(new ConfigDefinition("Interactables", "Remove Shrine of Shaping"), true, new ConfigDescription("Removes Shrine of Shaping.")).Value;
            NerfInteractableCredits.enabled = base.Config.Bind<bool>(new ConfigDefinition("Interactables", "Nerf Interactable Credits"), true, new ConfigDescription("Brings interactable credits in-line with Vanilla stages.")).Value;

            EarlygameSpawnPools.enabled = base.Config.Bind<bool>(new ConfigDefinition("Enemies", "Adjust Earlygame Spawnpools"), true, new ConfigDescription("Removes lategame enemies from earlygame stages.")).Value;

            Halcyonite.enabled = base.Config.Bind<bool>(new ConfigDefinition("Enemies", "Halcyonite - Hitboxes"), true, new ConfigDescription("Adjusts Halcyonite hitboxes, still kinda jank tho.")).Value;

            Child.removeIFrames = base.Config.Bind<bool>(new ConfigDefinition("Enemies", "Child - Remove I-Frames"), true, new ConfigDescription("Removes invulnerability from teleport.")).Value;
            Child.nerfDamage = base.Config.Bind<bool>(new ConfigDefinition("Enemies", "Child - Nerf Damage"), true, new ConfigDescription("Reduces projectile damage to be in line with other enmies.")).Value;

            Scorchling.removeIFrames = base.Config.Bind<bool>(new ConfigDefinition("Enemies", "Scorch Worm - Remove I-Frames"), true, new ConfigDescription("Removes invulnerability from burrow.")).Value;
            Scorchling.nerfDamage = base.Config.Bind<bool>(new ConfigDefinition("Enemies", "Scorch Worm - Nerf Damage"), true, new ConfigDescription("Reduces projectile damage a bit.")).Value;
        }

        private void ModCompat()
        {
            bool riskyTweaksLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.RiskyTweaks");
            bool riskyModLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.RiskyLives.RiskyMod");
            loopVariantPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("Wolfo.LoopVariantConfig");

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.RemoveReviveShrine"))
            {
                RemoveReviveShrine.enabled = false;
            }

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("Onyx.HalcyonKnight"))
            {
                Halcyonite.enabled = false;
            }

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Viliger.SpeedUpHalcyoniteShrine"))
            {
                HalcyonShrine.fasterCharging = false;
            }

            //Disable redundant tweaks
            if (riskyModLoaded)
            {
                //EarlygameSpawnPools.enabled = false;  //Reformed Altar changes need to run, check over there.
                Child.nerfDamage = false;
                Scorchling.nerfDamage = false;
            }

            //Disable redundant tweaks
            if (riskyTweaksLoaded)
            {
                Child.removeIFrames = false;
                Scorchling.removeIFrames = false;
            }
        }
    }
}
