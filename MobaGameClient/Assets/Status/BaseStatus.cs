using Framework.Character;
using FrogunnerGames.Stat;
using FrogunnerGames.Tag;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Status
{
    public abstract class BaseStatus : MonoBehaviour, ITaggable
    {
        [SerializeField] private Tagger _tagger;

        [SerializeField] private bool _permanent;

        [SerializeField] private bool _expirable = true;

        [SerializeField, ShowIf("_expirable"), InlineProperty, HideLabel, BoxGroup("Duration")]
        private Stat _duration;

        [SerializeField, BoxGroup("Stack Options")]
        private bool _override;

        [SerializeField, BoxGroup("Stack Options"), MinValue(0), HideIf("_override")]
        private int _maxLocalStack;

        [SerializeField, BoxGroup("Stack Options"), MinValue(0), HideIf("_override")]
        private int _maxGlobalStack = 10;

        [SerializeField] private BaseStatusPlacement _placement;

        public bool Permanent => _permanent;
        public ITagger Tagger => _tagger;

        public Transform Transform => _trans;

        public bool IsExpired { protected set; get; }

        public float Duration
        {
            set { _duration.BaseValue = value; }
            get { return _duration.Value; }
        }

        public int MaxLocalStack => _maxLocalStack;

        public int MaxGlobalStack => _maxGlobalStack;

        public bool Stackable => _maxLocalStack > 0;

        public bool Override => _override;

        public Actor Source { set; get; }
        public Actor Target { set; get; }

        public bool IsRunning { private set; get; }

        public float RemainingTimePercentage
        {
            get
            {
                if (Duration > 0f) return _durationTimer / _duration.Value;
                return 0f;
            }
        }

        public bool Expirable
        {
            set
            {
                _expirable = value;
                if (!_expirable)
                {
                    _durationTimer = 0f;
                }
            }
            get { return _expirable; }
        }

        private Transform _trans;
        private float _durationTimer;

        protected virtual void Awake()
        {
            _trans = transform;
        }

        public virtual bool ValidateAddingCondition(Actor owner, Actor target)
        {
            return true;
        }

        public virtual void SetActiveRenderer(bool active)
        {
        }

        public virtual void StartStatus()
        {
            IsExpired = false;
            IsRunning = true;
            _duration.RecalculateValue();
            _durationTimer = Duration;

            if (_placement != null)
                _placement.Place(_trans, Target);
        }

        public virtual void OnOverride()
        {
        }

        public void Cancel()
        {
            IsRunning = false;
            IsExpired = true;
            _duration.ClearModifiers();
        }

        public void Stop()
        {
            Cancel();
            End();
        }

        public void ResetDuration()
        {
            _durationTimer = Duration;
        }

        public void AddDurationModifier(StatModifier modifier)
        {
            _duration.AddModifier(modifier);
        }

        public void RemoveDurationModifier(StatModifier modifier)
        {
            _duration.RemoveModifier(modifier);
        }

        public virtual void AddDurationTimer(float amount)
        {
            _durationTimer = Mathf.Clamp(_durationTimer + amount, 0f, Duration);
        }

        public void Tick(float deltaTime)
        {
            if (!IsRunning || IsExpired) return;

            Execute();

            if (Expirable)
            {
                _durationTimer -= Time.deltaTime;

                if (_durationTimer <= 0f)
                {
                    IsRunning = false;
                    IsExpired = true;
                }
            }
        }

        protected virtual void Execute()
        {
        }

        protected virtual void End()
        {
        }
    }
}