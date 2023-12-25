using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FrogunnerGames.Stat
{
    [Serializable, InlineProperty, HideLabel]
    public class Stat
    {
        [SerializeField] private ObscuredFloat _baseValue;

        public float BaseValue
        {
            set
            {
                value = Mathf.Clamp(value, _minValue, _maxValue);

                if (Math.Abs(_baseValue - value) > Mathf.Epsilon)
                {
                    _baseValue = value;
                    RecalculateValue();
                }
            }
            get { return _baseValue; }
        }

        [SerializeField, ReadOnly] private ObscuredFloat _value;
        [SerializeField] private float _minValue = 0f;
        [SerializeField] private float _maxValue = float.MaxValue;

//        private bool isDirty = true;
//        private float lastBaseValue;

        public float LastValue { private set; get; }
        public float Value => _value;
        public float ConstraintMin => _minValue;
        public float ConstraintMax => _maxValue;

        public IEnumerable<StatModifier> Modifiers => attributeModifiers;

        [SerializeField] private List<StatModifier> attributeModifiers;

        //protected readonly List<AttributeModifier> attributeModifiers;
        //public readonly ReadOnlyCollection<AttributeModifier> AttributeModifiers;

        private float _lastValue;
        private readonly List<Action<float>> _listeners;

        public Stat()
        {
            //AttributeModifiers = attributeModifiers.AsReadOnly();
            attributeModifiers = new List<StatModifier>();
            _listeners = new List<Action<float>>();
        }

        public Stat(float baseValue) : this()
        {
            BaseValue = baseValue;
        }

        public Stat(float baseValue, float min, float max) : this(baseValue)
        {
            SetConstraintMin(min);
            SetConstraintMax(max);
        }

        public void SetConstraintMin(float min)
        {
            if (min > _maxValue)
            {
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                Debug.LogError("Min value " + min + " cannot be greater than Max value  " + _maxValue);
                return;
            }

            _minValue = min;
        }

        public void SetConstraintMax(float max)
        {
            if (max < _minValue)
            {
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                Debug.LogError("Max value " + max + " cannot be smaller than Min value  " + _minValue);
                return;
            }

            _maxValue = max;
        }

        public void AddListener(Action<float> callback)
        {
            _listeners.Add(callback);
            callback(Value);
        }

        public void RemoveListener(Action<float> callback)
        {
            _listeners.Remove(callback);
        }

        public void ClearAllListeners()
        {
            _listeners.Clear();
        }

        public virtual StatModifier GetModifier(int index)
        {
            return attributeModifiers[index];
        }

        public virtual void AddModifier(StatModifier mod)
        {
//            isDirty = true;
            mod.Stat = this;
            attributeModifiers.Add(mod);
            attributeModifiers.Sort(CompareModifierOrder);
            RecalculateValue();
        }

        public virtual bool RemoveModifier(StatModifier mod)
        {
            if (!attributeModifiers.Remove(mod)) return false;
            mod.Stat = null;
//            isDirty = true;
            RecalculateValue();
            return true;
        }

        public bool HasModifier(StatModifier modifier)
        {
            return attributeModifiers.Contains(modifier);
        }

        public void ClearModifiers()
        {
            attributeModifiers.Clear();
            RecalculateValue();
        }

        public virtual bool RemoveAllModifiersFromSource(object source)
        {
            var didRemove = false;

            for (int i = attributeModifiers.Count - 1; i >= 0; i--)
            {
                if (attributeModifiers[i].Source != source) continue;
                attributeModifiers[i].Stat = null;
//                isDirty = true;
                didRemove = true;
                attributeModifiers.RemoveAt(i);
            }

            RecalculateValue();
            return didRemove;
        }

        public virtual int CountModifiersFromSource(object source)
        {
            var count = 0;
            for (int i = attributeModifiers.Count - 1; i >= 0; i--)
            {
                if (attributeModifiers[i].Source == source) count++;
            }

            return count;
        }

        protected virtual int CompareModifierOrder(StatModifier a, StatModifier b)
        {
            if (a.Order < b.Order)
                return -1;
            else if (a.Order > b.Order) return 1;
            return 0; //if (a.Order == b.Order)
        }

        /// <summary>
        /// Follow formula: Value = Sum(Flat) x (1 + Sum(Increase) - Sum(Reduce)) x Product(1 + More) x Product(1 - Less)
        /// </summary>
        /// <param name="baseValue"></param>
        /// <returns></returns>
        protected virtual float CalculateFinalValue(float baseValue)
        {
            float finalValue = baseValue;
            float sumPercentAdd = 0;

            for (int i = 0; i < attributeModifiers.Count; i++)
            {
                StatModifier mod = attributeModifiers[i];

                switch (mod.Type)
                {
                    case StatModType.Flat:
                        finalValue += mod.Value;
                        break;
                    case StatModType.PercentAdd:
                    {
                        sumPercentAdd += mod.Value;

                        if (i + 1 >= attributeModifiers.Count || attributeModifiers[i + 1].Type != StatModType.PercentAdd)
                        {
                            finalValue *= 1 + sumPercentAdd;
                            sumPercentAdd = 0;
                        }

                        break;
                    }

                    case StatModType.PercentMul:
                        finalValue *= 1 + mod.Value;
                        break;
                }
            }

            // Workaround for float calculation errors, like displaying 12.00002 instead of 12
            return (float) Math.Round(finalValue, 2);
        }

        /// <summary>
        /// Follow formula: Value = Sum(Flat) x (1 + Sum(Increase) - Sum(Reduce)) x Sum(1 + More) x Sum(1 - Less)
        /// </summary>
        /// <param name="baseValue"></param>
        /// <returns></returns>
        protected virtual float CalculateFinalValuePOE(float baseValue)
        {
            var finalValue = baseValue;
            var sumPercentAdd = 0f;
            var sumPercentMulMore = 0f;
            var sumPercentMulLess = 0f;

            for (int i = 0; i < attributeModifiers.Count; i++)
            {
                StatModifier mod = attributeModifiers[i];

                switch (mod.Type)
                {
                    case StatModType.Flat:
                        finalValue += mod.Value;
                        break;
                    case StatModType.PercentAdd:
                    {
                        sumPercentAdd += mod.Value;
                        break;
                    }

                    case StatModType.PercentMul:

                        // More
                        if (mod.Value >= 0f)
                        {
                            sumPercentMulMore += mod.Value;
                        }
                        // Less
                        else
                        {
                            sumPercentMulLess += mod.Value;
                        }

                        break;
                }
            }

            // Percent Add (Increase, Decrease)
            finalValue *= 1f + sumPercentAdd;

            // Percent Mul (More, Less)
            finalValue *= 1f + sumPercentMulMore;
            finalValue *= 1f + sumPercentMulLess;

            // Workaround for float calculation errors, like displaying 12.00002 instead of 12
            return finalValue;
//            return (float) System.Math.Round(finalValue, 4);
        }

        public void RecalculateValue()
        {
//            lastBaseValue = _baseValue;
            LastValue = _value;
            float baseValue = _baseValue;
            _value = attributeModifiers != null ? CalculateFinalValuePOE(baseValue) : baseValue;
            _value = Mathf.Clamp(_value, _minValue, _maxValue);

            if (Math.Abs(_value - _lastValue) > 1e-4)
            {
                _lastValue = _value;
                InvokeListeners();
            }
        }

        public float CalculateTemporaryValue(float baseValue)
        {
            float value = CalculateFinalValuePOE(baseValue);
            value = Mathf.Clamp(value, _minValue, _maxValue);
            return value;
        }

        private void InvokeListeners()
        {
            foreach (var listener in _listeners)
            {
                listener?.Invoke(_value);
            }
        }

        public override string ToString()
        {
            return
                $"{nameof(_baseValue)}: {_baseValue}, {nameof(_value)}: {_value}, {nameof(_minValue)}: {_minValue}, {nameof(_maxValue)}: {_maxValue}, {nameof(attributeModifiers)} count: {attributeModifiers.Count}";
        }
    }
}