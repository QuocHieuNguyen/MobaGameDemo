using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Status;
using UnityEngine;

namespace Framework.Character
{
    public class NullStatusEngine : IStatusEngine
    {
        public NullStatusEngine()
        {
        }

        public bool IsLocked
        {
            set { }
            get { return true; }
        }

        public bool UsingStatusThreshold { get; set; }

        public IList<BaseStatus> Statuses
        {
            get { return Enumerable.Empty<BaseStatus>() as IList<BaseStatus>; }
        }

        public bool HasStatus(Actor source)
        {
            return true;
        }

        public void ClearAllStatus(bool forced = false)
        {
        }

        public void ClearStatuses(string tag, bool forced = false)
        {
        }

        public void ClearStatus(BaseStatus status, bool forced = false)
        {
        }

        public void ClearStatuses<T>(bool forced = false) where T : BaseStatus
        {
        }

        public void ClearStatuses(Type type, bool forced = false)
        {
        }

        public void ClearStatuses<T>(Actor source, bool forced = false) where T : BaseStatus
        {
        }

        public void ClearStatuses(Actor source, bool forced = false)
        {
        }

        public void AddStatuses(Actor source, GameObject[] statuses)
        {
        }

        public BaseStatus AddStatus(Actor source, GameObject statusPrefab, bool forced = false)
        {
            return null;
        }

        public bool TryAddStatus(Actor source, GameObject statusPrefab, out BaseStatus status, bool forced = false)
        {
            status = null;
            return false;
        }

        public bool TryAddStatusDeferredStart(Actor source, GameObject statusPrefab, out BaseStatus status, bool forced = false)
        {
            status = null;
            return false;
        }

        public bool IsImmune(string tag)
        {
            return false;
        }

        public bool IsImmune(IList<string> tags)
        {
            return false;
        }

        public int CountStatus(Type type)
        {
            return 0;
        }

        public int CountStatus<T>() where T : BaseStatus
        {
            return 0;
        }

        public int CountStatus(Type type, Actor source)
        {
            return 0;
        }

        public int CountStatus<T>(Actor source) where T : BaseStatus
        {
            return 0;
        }

        public bool HasStatusWithTag(string tag)
        {
            return false;
        }

        public T GetStatus<T>() where T : BaseStatus
        {
            return default(T);
        }

        public T GetStatus<T>(Actor source) where T : BaseStatus
        {
            return default;
        }

        public bool HasStatus<T>() where T : BaseStatus
        {
            return false;
        }

        public bool HasStatus<T>(Actor source) where T : BaseStatus
        {
            return false;
        }

        public bool HasStatus(Type type)
        {
            return false;
        }

        public bool IsImmune(Type type)
        {
            return true;
        }

        public void SetImmune(string tag, bool immune)
        {
        }

        public void LateTick(float deltaTime)
        {
            
        }

        public void SetActiveRenderer(bool active)
        {
        }

        public void SetImmune<T>(bool immune) where T : BaseStatus
        {
        }

        public void SetImmune(Type type, bool immune)
        {
        }

        public void Initialize(Actor actor)
        {
        }

        public void Tick(float deltaTime)
        {
        }
    }
}