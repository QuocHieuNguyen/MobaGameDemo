using System;
using Framework.Character;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework
{
    public abstract class BaseStatusThreshold : MonoBehaviour
    {
        [SerializeField, MinValue(0f)] private float _threshold;

        [SerializeField] private StatusPoint[] _statusPoints;

        private float _point;

        protected bool Active { set; get; }
        protected Actor Actor { private set; get; }

        public void Initialize(Actor actor)
        {
            Actor = actor;
        }

        public void AddPoint(float point)
        {
            if (Active) return;

            _point += point;

            if (_point >= _threshold)
            {
                _point = 0f;
                Active = true;
                ReachThreshold();
            }
        }

        public void Tick(float deltaTime)
        {
            if (Active)
            {
                OnActive(deltaTime);
            }
        }

        public bool HasStatusType(Type type, out StatusPoint pointData)
        {
            pointData = null;
            foreach (var statusPoint in _statusPoints)
            {
                if (statusPoint.StatusType == type)
                {
                    pointData = statusPoint;
                    return true;
                }
            }

            return false;
        }

        public abstract void OnActive(float deltaTime);
        protected abstract void ReachThreshold();
    }
}