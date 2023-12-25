using System;
using System.Text;
using CodeStage.AntiCheat.ObscuredTypes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FrogunnerGames.Stat
{
    [Serializable]
    public class StatModifier
    {
        [SerializeField, HideInInspector] private ObscuredFloat _value;

        [SerializeField] private StatModType _type;

        [ShowInInspector]
        public float Value
        {
            set
            {
                _value = value;
                _stat?.RecalculateValue();
            }
            get => _value;
        }

        public StatModType Type => _type;

        [HideInInspector] public int Order;
        [HideInInspector] public object Source;
        
        [HideInEditorMode][ShowInInspector] public string SourceString
        {
            get
            {
                var source = (Source as UnityEngine.Object);
                if (source != null)
                    return source.ToString();
                return Source?.ToString();
            }
            set
            {
                //ignore
            }
        }
        
        [NonSerialized] private Stat _stat;

        public Stat Stat
        {
            set { _stat = value; }
            get { return _stat; }
        }

        public StatModifier(float value, StatModType type, int order, object source)
        {
            Value = value;
            _type = type;
            Order = order;
            Source = source;
        }

        // public StatModifier(float value, StatModType type) : this(value, type, (int) type)
        // {
        // }

        public StatModifier(float value, StatModType type, object source) : this(value, type, (int) type, source)
        {
        }

        public static StatModType StringToType(string modType)
        {
            switch (modType)
            {
                case "Flat": return StatModType.Flat;
                case "PercentAdd": return StatModType.PercentAdd;
                case "PercentMult": return StatModType.PercentMul;
            }

            return StatModType.Flat;
        }

        public override string ToString()
        {
            // return $"{nameof(_value)}: {_value}, {nameof(_type)}: {_type}, {nameof(Order)}: {Order}, {nameof(Source)}: {Source}, {nameof(_stat)}: {_stat}";
            return $"{nameof(_value)}: {_value}, {nameof(_type)}: {_type}, {nameof(Order)}: {Order}, {nameof(_stat)}: {_stat}";
        }

        public StatModifier Clone()
        {
            return new StatModifier(_value, _type, _stat);
        }
    }
}