using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuffFactorySystem
{
    [CreateAssetMenu(menuName = "Custom/Data/Buff/Strength Potion")]
    public class StrengthPotionBuffFactory : BuffFactory<StrengthPotionBuffData, StrengthPotionBuff>
    {

    }
    [System.Serializable]
    public class StrengthPotionBuffData
    {
        public int strengthToAdd = 5;
        public float duration = 60f;
    }

    public class StrengthPotionBuff: Buff<StrengthPotionBuffData>
    {
        public override void Apply()
        {
            target.AddStrength(data.strengthToAdd);
            target.StartCoroutine(UnapplicationCoroutine());
        }

        public IEnumerator UnapplicationCoroutine()
        {
            yield return new WaitForSeconds(data.duration);
            target.RemoveStrength(data.strengthToAdd);
        }
    }
}