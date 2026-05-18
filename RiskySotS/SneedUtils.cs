using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskySotS
{
    public static class SneedUtils
    {
        private static SceneDef bazaarSceneDef = Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/bazaar/bazaar.asset").WaitForCompletion();
        public static bool IsInBazaar()
        {
            SceneDef sd = SceneCatalog.GetSceneDefForCurrentScene();
            return sd && sd == bazaarSceneDef;
        }

        public static bool SetAddressableEntityStateField(string fullEntityStatePath, string fieldName, string value)
        {
            EntityStateConfiguration esc = Addressables.LoadAssetAsync<EntityStateConfiguration>(fullEntityStatePath).WaitForCompletion();
            for (int i = 0; i < esc.serializedFieldsCollection.serializedFields.Length; i++)
            {
                if (esc.serializedFieldsCollection.serializedFields[i].fieldName == fieldName)
                {
                    esc.serializedFieldsCollection.serializedFields[i].fieldValue.stringValue = value;
                    return true;
                }
            }
            return false;
        }

        public static bool SetAddressableEntityStateField(string fullEntityStatePath, string fieldName, Object value)
        {
            EntityStateConfiguration esc = Addressables.LoadAssetAsync<EntityStateConfiguration>(fullEntityStatePath).WaitForCompletion();
            for (int i = 0; i < esc.serializedFieldsCollection.serializedFields.Length; i++)
            {
                if (esc.serializedFieldsCollection.serializedFields[i].fieldName == fieldName)
                {
                    esc.serializedFieldsCollection.serializedFields[i].fieldValue.objectValue = value;
                    return true;
                }
            }
            return false;
        }

        //Minibosses
        //Basic Monsters
        //Champions
        public enum MonsterCategories
        {
            BasicMonsters, Minibosses, Champions
        }

        public static int FindCategoryIndexByName(DirectorCardCategorySelection dcs, MonsterCategories category)
        {
            string categoryName;
            switch (category)
            {
                case MonsterCategories.BasicMonsters:
                    categoryName = "Basic Monsters";
                    break;
                case MonsterCategories.Minibosses:
                    categoryName = "Minibosses";
                    break;
                case MonsterCategories.Champions:
                    categoryName = "Champions";
                    break;
                default:
                    return -1;
            }
            return FindCategoryIndexByName(dcs, categoryName);
        }

        public static int FindCategoryIndexByName(DirectorCardCategorySelection dcs, string categoryName)
        {
            //Debug.Log("Dumping categories for " + dcs.name);
            for (int i = 0; i < dcs.categories.Length; i++)
            {
                //Debug.Log(dcs.categories[i].name);
                if (string.CompareOrdinal(dcs.categories[i].name, categoryName) == 0)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
