using System;
using System.Collections.Generic;
using Framework.Character;
using FrogunnerGames.EventSystem;
using Sirenix.OdinInspector;
using TypeReferences;
using UnityEngine;

namespace Framework.Status
{
    [DisallowMultipleComponent]
    public class StatusEngine : MonoBehaviour, IStatusEngine
    {
        [SerializeField, Range(0f, 1f)] private float _negativeStatusDurationMul = 1f;

        [SerializeField] private bool _immuneNegativeStatus;

        [SerializeField, ClassExtends(typeof(BaseStatus))]
        private ClassTypeReference[] _immuneStatuses;

        [SerializeField] private BaseStatusThreshold[] _statusThresholds;

        private Actor _actor;
        private List<string> _immuneTags;
        private Transform _trans;
        private List<BaseStatus> _statuses;

        public bool IgnoreNegativeStatus
        {
            set { _immuneNegativeStatus = value; }
            get { return _immuneNegativeStatus; }
        }

        public float NegativeStatusDurationMul
        {
            set { _negativeStatusDurationMul = value; }
            get { return _negativeStatusDurationMul; }
        }

        [ShowInInspector] public bool UsingStatusThreshold { set; get; }

        public IList<BaseStatus> Statuses => _statuses;

        public bool IsLocked { set; get; }

        public void Initialize(Actor actor)
        {
            _actor = actor;
            _trans = actor.CachedTransform;
            _statuses = new List<BaseStatus>();
            _immuneTags = new List<string>();

            foreach (var immuneType in _immuneStatuses)
            {
                SetImmune(immuneType.Type, true);
            }

            if (_statusThresholds != null)
            {
                foreach (var statusThreshold in _statusThresholds)
                {
                    statusThreshold.Initialize(actor);
                }
            }
        }

        #region PUBLIC

        public void Tick(float deltaTime)
        {
            foreach (var status in _statuses)
            {
                status.Tick(deltaTime);
            }

            if (UsingStatusThreshold)
            {
                foreach (var statusThreshold in _statusThresholds)
                {
                    statusThreshold.Tick(deltaTime);
                }
            }
        }

        public void LateTick(float deltaTime)
        {
            for (int i = _statuses.Count - 1; i >= 0; i--)
            {
                if (i < 0 || i >= _statuses.Count) break;

                var status = _statuses[i];
                if (status == null)
                {
                    _statuses.RemoveAt(i);
                    continue;
                }

                if (!status.IsExpired) continue;
                status.Stop();
                _statuses.RemoveAt(i);
                Messenger.Broadcast(EventKey.RemoveStatus, _actor, status);
                PoolFactory.Despawn(status.gameObject);
            }
        }

        public void SetActiveRenderer(bool active)
        {
            foreach (var status in _statuses) status.SetActiveRenderer(active);
        }

        public void SetImmune<T>(bool immune) where T : BaseStatus
        {
            var type = typeof(T);

            if (immune)
            {
                _immuneTags.Add(type.Name);
            }
            else
            {
                _immuneTags.Remove(type.Name);
            }
        }

        public void SetImmune(Type type, bool immune)
        {
            if (immune)
            {
                _immuneTags.Add(type.Name);
            }
            else
            {
                _immuneTags.Remove(type.Name);
            }
        }

        public void SetImmune(string tag, bool immune)
        {
            if (immune)
            {
                _immuneTags.Add(tag);
            }
            else
            {
                _immuneTags.Remove(tag);
            }
        }

        public bool IsImmune(Type type)
        {
            return _immuneTags.Contains(type.Name);
        }

        public bool IsImmune(string tag)
        {
            return _immuneTags.Contains(tag);
        }

        public bool IsImmune(IList<string> tags)
        {
            if (tags.Count == 0) return false;

            foreach (var tag in tags)
            {
                if (!_immuneTags.Contains(tag)) return false;
            }

            return true;
        }

        public int CountStatus(Type type)
        {
            int count = 0;
            foreach (var status in _statuses)
            {
                if (status.GetType() == type)
                {
                    count++;
                }
            }

            return count;
        }

        public int CountStatus<T>() where T : BaseStatus
        {
            int count = 0;
            foreach (var status in _statuses)
            {
                if (status.GetType() == typeof(T))
                {
                    count++;
                }
            }

            return count;
        }

        public int CountStatus(Type type, Actor source)
        {
            int count = 0;
            foreach (var status in _statuses)
            {
                if (status.Source == source && status.GetType() == type)
                {
                    count++;
                }
            }

            return count;
        }

        public int CountStatus<T>(Actor source) where T : BaseStatus
        {
            var type = typeof(T);
            var count = 0;
            foreach (var status in _statuses)
            {
                if (status.Source == source && status.GetType() == type)
                {
                    count++;
                }
            }

            return count;
        }

        public bool HasStatusWithTag(string tag)
        {
            foreach (var status in _statuses)
            {
                if (status != null && status.Tagger.HasTag(tag)) return true;
            }

            return false;
        }

        public T GetStatus<T>() where T : BaseStatus
        {
            foreach (var status in _statuses)
            {
                if (status.GetType() == typeof(T))
                {
                    return (T) status;
                }
            }

            return default(T);
        }

        public T GetStatus<T>(Actor source) where T : BaseStatus
        {
            var type = typeof(T);
            foreach (var status in _statuses)
            {
                if (status.GetType() == type && status.Source == source)
                {
                    return (T) status;
                }
            }

            return default(T);
        }

        public bool HasStatus<T>() where T : BaseStatus
        {
            foreach (var status in _statuses)
            {
                if (status.GetType() == typeof(T))
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasStatus<T>(Actor source) where T : BaseStatus
        {
            foreach (var status in _statuses)
            {
                if (status.GetType() == typeof(T) && status.Source == source)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasStatus(Type type)
        {
            foreach (var status in _statuses)
            {
                if (status.GetType() == type)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasStatus(Actor source)
        {
            foreach (var status in _statuses)
            {
                if (status.Source == source)
                {
                    return true;
                }
            }

            return false;
        }

        public void ClearAllStatus(bool forced = false)
        {
            for (int i = _statuses.Count - 1; i >= 0; --i)
            {
                if (i >= _statuses.Count) break;

                var status = _statuses[i];

                if (status == null) continue;
                if (forced)
                {
                    status.Stop();
                }
                else
                {
                    if (!status.Permanent)
                    {
                        status.Stop();
                    }
                }
            }
        }

        public void ClearStatuses(string tag, bool forced = false)
        {
            for (int i = _statuses.Count - 1; i >= 0; --i)
            {
                if (i >= _statuses.Count) break;

                BaseStatus status = _statuses[i];

                if (!status.Tagger.HasTag(tag)) continue;
                if (forced)
                {
                    status.Stop();
                }
                else
                {
                    if (!status.Permanent)
                    {
                        status.Stop();
                    }
                }
            }
        }

        public void ClearStatus(BaseStatus status, bool forced = false)
        {
            if (forced)
            {
                status.Stop();
            }
            else
            {
                if (!status.Permanent)
                {
                    status.Stop();
                }
            }
        }

        /// <summary>
        /// Clear all statuses of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void ClearStatuses<T>(bool forced = false) where T : BaseStatus
        {
            for (int i = _statuses.Count - 1; i >= 0; --i)
            {
                if (i >= _statuses.Count) break;

                BaseStatus status = _statuses[i];

                if (status.GetType() != typeof(T)) continue;
                if (forced)
                {
                    status.Stop();
                    _statuses.RemoveAt(i);
                    PoolFactory.Despawn(status.gameObject);
                }
                else
                {
                    if (!status.Permanent)
                    {
                        status.Stop();
                        _statuses.RemoveAt(i);
                        PoolFactory.Despawn(status.gameObject);
                    }
                }
            }
        }

        /// <summary>
        /// Clear all statuses of a type
        /// </summary>
        /// <param name="type"></param>
        public void ClearStatuses(Type type, bool forced = false)
        {
            for (int i = _statuses.Count - 1; i >= 0; --i)
            {
                if (i >= _statuses.Count) break;

                BaseStatus status = _statuses[i];

                if (status.GetType() != type) continue;
                if (forced)
                {
                    status.Stop();
                }
                else
                {
                    if (!status.Permanent)
                    {
                        status.Stop();
                    }
                }
            }
        }

        public void ClearStatuses<T>(Actor source, bool forced = false) where T : BaseStatus
        {
            for (int i = _statuses.Count - 1; i >= 0; --i)
            {
                if (i >= _statuses.Count) break;

                BaseStatus status = _statuses[i];

                if (status.GetType() != typeof(T) || source != status.Source) continue;
                if (forced)
                {
                    status.Stop();
                }
                else
                {
                    if (!status.Permanent)
                    {
                        status.Stop();
                    }
                }
            }
        }

        public void ClearStatuses(Type type, Actor source, bool forced = false)
        {
            for (int i = _statuses.Count - 1; i >= 0; --i)
            {
                if (i >= _statuses.Count) break;

                BaseStatus status = _statuses[i];

                if (status.GetType() != type || source != status.Source) continue;
                if (forced)
                {
                    status.Stop();
                }
                else
                {
                    if (!status.Permanent)
                    {
                        status.Stop();
                    }
                }
            }
        }

        /// <summary>
        /// Clear all statuses caused by source id
        /// </summary>
        /// <param name="sourceId"></param>
        public void ClearStatuses(Actor source, bool forced = false)
        {
            for (int i = _statuses.Count - 1; i >= 0; --i)
            {
                if (i >= _statuses.Count) break;

                BaseStatus status = _statuses[i];

                if (status.Source != source) continue;
                if (forced)
                {
                    status.Stop();
                }
                else
                {
                    if (!status.Permanent) status.Stop();
                }
            }
        }

        /// <summary>
        /// Add all statuses caused by source id
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="statuses"></param>
        public void AddStatuses(Actor source, GameObject[] statuses)
        {
            if (IsLocked) return;

            foreach (var status in statuses)
            {
                if (status != null) AddStatus(source, status);
            }
        }

        /// <summary>
        /// Add a status caused by source id
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="statusPrefab"></param>
        [Button]
        public BaseStatus AddStatus(Actor source, GameObject statusPrefab, bool forced = false)
        {
            if (!TryCreateStatus(source, statusPrefab, out BaseStatus addedStatus, forced)) return null;
            addedStatus.StartStatus();
            return addedStatus;
        }

        public bool TryAddStatus(Actor source, GameObject statusPrefab, out BaseStatus status, bool forced = false)
        {
            status = AddStatus(source, statusPrefab, forced);
            return status != null;
        }

        /// <summary>
        /// Defer from calling start status. The method StartStatus need to be called explicitly
        /// </summary>
        /// <param name="source"></param>
        /// <param name="statusPrefab"></param>
        /// <param name="status"></param>
        /// <param name="forced"></param>
        /// <returns></returns>
        public bool TryAddStatusDeferredStart(Actor source, GameObject statusPrefab, out BaseStatus status, bool forced = false)
        {
            status = null;
            if (!TryCreateStatus(source, statusPrefab, out BaseStatus addedStatus, forced))
            {
                return false;
            }

            status = addedStatus;
            return true;
        }

        private bool TryCreateStatus(Actor source, GameObject statusPrefab, out BaseStatus addedStatus, bool forced = false)
        {
            addedStatus = null;

            if (IsLocked) return false;
            if (statusPrefab == null)
            {
                Debug.LogError("Status Prefab is null " + source.gameObject.name);
                return false;
            }

            var statusTemplate = statusPrefab.GetComponent<BaseStatus>();

            if (!statusTemplate.ValidateAddingCondition(source, _actor))
            {
                return false;
            }

            Type statusType = statusTemplate.GetType();

            int currentGlobalStack = CountStatus(statusType);
            if (currentGlobalStack > statusTemplate.MaxGlobalStack) return false;

            if (!forced && (IsImmune(statusType) || IsImmune(statusTemplate.Tagger.Tags)))
            {
#if UNITY_EDITOR
                //Debug.Log(gameObject.name + "immunes to status " + statusTemplate.GetType());
#endif
                return false;
            }

            if (statusTemplate.Override)
            {
                ClearStatuses(statusType, source);
                addedStatus = CreateStatusInstance(statusPrefab, source);
                if (UsingStatusThreshold) ApplyStatusThreshold(addedStatus);
            }
            else if (statusTemplate.Stackable)
            {
                int currentStack = CountStatus(statusType, source);

                if (currentStack < statusTemplate.MaxLocalStack)
                {
                    addedStatus = CreateStatusInstance(statusPrefab, source);
                    if (UsingStatusThreshold) ApplyStatusThreshold(addedStatus);
                    return true;
                }
            }
            else
            {
                foreach (var status in _statuses)
                {
//                    if (status == null || status.Source != source || status.GetType() != statusType) continue;
                    if (status == null || status.GetType() != statusType) continue;

                    status.Source = source;
                    status.OnOverride();
                    status.ResetDuration();
                    addedStatus = status;
                    if (UsingStatusThreshold) ApplyStatusThreshold(addedStatus);
                    return false;
                }

                addedStatus = CreateStatusInstance(statusPrefab, source);
                if (UsingStatusThreshold) ApplyStatusThreshold(addedStatus);
                return true;
            }

            return false;
        }

        private BaseStatus CreateStatusInstance(GameObject statusPrefab, Actor source)
        {
            var addedStatus = PoolFactory.Spawn<BaseStatus>(statusPrefab, Vector3.zero, Quaternion.identity);
            Transform statusTransform = addedStatus.Transform;
            statusTransform.SetParent(_trans);
            statusTransform.localPosition = Vector3.zero;
            statusTransform.localRotation = Quaternion.identity;
            addedStatus.Source = source;
            addedStatus.Target = _actor;
            _statuses.Add(addedStatus);
            Messenger.Broadcast(EventKey.AddStatus, source, _actor, addedStatus);
            return addedStatus;
        }

        private void ApplyStatusThreshold(BaseStatus status)
        {
            foreach (var statusThreshold in _statusThresholds)
            {
                if (!statusThreshold.HasStatusType(status.GetType(), out StatusPoint statusPoint)) continue;
                statusThreshold.AddPoint(statusPoint.Point);
            }
        }

        #endregion
    }
}