using RoR2;
using UnityEngine;

namespace RiskySotS.Tweaks.Interactables
{
    public class NerfInteractableCredits
    {
        public static bool enabled;

        public NerfInteractableCredits()
        {
            if (!enabled) return;
            SceneDirector.onPrePopulateSceneServer += SceneDirector_onPrePopulateSceneServer;
        }

        private void SceneDirector_onPrePopulateSceneServer(SceneDirector director)
        {
            SceneDef currentScene = SceneCatalog.GetSceneDefForCurrentScene();
            if (!currentScene) return;

            switch (currentScene.baseSceneName)
            {
                case "village":
                    director.interactableCredit = 240;  //280 -> 240
                    break;
                case "villagenight":
                    director.interactableCredit = 210;   //240-30 for guaranteed large chest
                    break;
                case "lakes":
                    director.interactableCredit = 240;  //280 -> 240
                    break;
                case "lakesnight":
                    director.interactableCredit = 40; //280 -> 240
                    break;
                case "lemuriantemple":
                    director.interactableCredit  = 220;//300 -> 220, matches Abandoned Aqueduct; this is a small map.
                    break;
                /*case "habitat":   //280, this is standard
                    break;
                case "habitatfall":   //280, this is standard
                    break;
                case "helminthroost":   //570, higher than Sky Meadow's 520 but the map size justifies it
                    break;*/
                default:
                    break;
            }
        }
    }
}
