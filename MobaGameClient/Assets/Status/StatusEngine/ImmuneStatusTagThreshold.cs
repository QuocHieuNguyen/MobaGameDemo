using FrogunnerGames.Inspector;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework
{
    public class ImmuneStatusTagThreshold : BaseStatusThreshold
    {
        [SerializeField, MinValue(0f)] private float _duration;
        [SerializeField, TagList] private string[] _tags;

        private float _timer;

        public override void OnActive(float deltaTime)
        {
            _timer -= deltaTime;

            if (_timer <= 0f)
            {
                _timer = _duration;
                Active = false;

                var statusEngine = Actor.StatusEngine;
                foreach (var tag in _tags)
                {
                    statusEngine.SetImmune(tag, false);
                }
            }
        }

        protected override void ReachThreshold()
        {
            _timer = _duration;

            var statusEngine = Actor.StatusEngine;
            foreach (var tag in _tags)
            {
                statusEngine.SetImmune(tag, true);
            }
        }
    }
}