using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskySotS.Tweaks.Enemies
{
    public class Halcyonite
    {
        public static bool enabled;

        public Halcyonite()
        {
            if (!enabled) return;

            GameObject bodyObject = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/Halcyonite/HalcyoniteBody.prefab").WaitForCompletion();
            var modelObject = bodyObject.GetComponent<ModelLocator>().modelTransform.gameObject;

            //Jank, doesn't work well
            /*ChildLocator cl = modelObject.GetComponent<ChildLocator>();
            Transform swordPoint = cl.FindChild("SwordPoint");
            Transform weapon = swordPoint.parent;
            GameObject actualSword = new GameObject();
            actualSword.transform.SetParent(weapon);
            actualSword.transform.localPosition = new Vector3(2f, 0f, 0f);
            actualSword.transform.localScale = new Vector3(7f, 2f, 1f);
            actualSword.transform.localRotation = Quaternion.identity;
            HitBox actualHitbox = actualSword.gameObject.AddComponent<HitBox>();*/

            HitBoxGroup[] hbg = modelObject.GetComponentsInChildren<HitBoxGroup>();

            bool modifiedGoldenSword = false, modifiedGoldenSlash = false;
            for (int i = 0; i < hbg.Length; i++)
            {
                var hb = hbg[i];
                if (hb.groupName == "GoldenSword" && !modifiedGoldenSword)
                {
                    //Poke
                    modifiedGoldenSword = true;
                    var hit = hb.hitBoxes[0];
                    hit.transform.localPosition = new Vector3(1f, 3f, 7f);
                    hit.transform.localScale = new Vector3(3f, 3f, 12f);
                }
                else if (hb.groupName == "GoldenSlash" && !modifiedGoldenSlash)
                {
                    //Slash
                    modifiedGoldenSlash = true;
                    var hit = hb.hitBoxes[0];
                    hit.transform.localPosition = new Vector3(0f, 1.6f, 4f);
                    hit.transform.localScale = new Vector3(12f, 3f, 9f);
                }
            }
        }
    }
}
