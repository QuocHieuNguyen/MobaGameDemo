using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuffFactorySystem
{
    public abstract class BuffFactory : ScriptableObject
    {
        public abstract Buff GetBuff(Character target);
    }

    public class BuffFactory<DataType, BuffType> : BuffFactory where BuffType: Buff<DataType>, new()
    {
        public DataType data;
        
        public override Buff GetBuff(Character target)
        {
            return new BuffType { data = this.data, target = target };
        }
    }

    public abstract class Buff
    {
        public abstract void Apply();
    }
    public abstract class Buff<DataType>: Buff
    {
        public DataType data;
        public Character target;
        
    }
}