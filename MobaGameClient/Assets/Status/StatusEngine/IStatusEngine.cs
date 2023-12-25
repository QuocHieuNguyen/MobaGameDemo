using System;
using System.Collections.Generic;
using Framework.Status;
using UnityEngine;

namespace Framework.Character
{
    public interface IStatusEngine
    {
        bool IsLocked { set; get; }
        bool UsingStatusThreshold { set; get; }
        IList<BaseStatus> Statuses { get; }

        void Initialize(Actor actor);

        void Tick(float deltaTime);

        void LateTick(float deltaTime);

        void SetActiveRenderer(bool active);

        void SetImmune<T>(bool immune) where T : BaseStatus;

        void SetImmune(Type type, bool immune);

        bool IsImmune(Type type);

        void SetImmune(string tag, bool immune);

        bool IsImmune(string tag);

        bool IsImmune(IList<string> tags);

        int CountStatus(Type type);

        int CountStatus<T>() where T : BaseStatus;

        int CountStatus(Type type, Actor source);

        int CountStatus<T>(Actor source) where T : BaseStatus;

        bool HasStatusWithTag(string tag);

        T GetStatus<T>() where T : BaseStatus;

        T GetStatus<T>(Actor source) where T : BaseStatus;

        bool HasStatus<T>() where T : BaseStatus;

        bool HasStatus<T>(Actor source) where T : BaseStatus;

        bool HasStatus(Type type);

        bool HasStatus(Actor source);

        void ClearStatus(BaseStatus status, bool forced = false);

        void ClearAllStatus(bool forced = false);

        void ClearStatuses(string tag, bool forced = false);

        /// <summary>
        /// Clear all statuses of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void ClearStatuses<T>(bool forced = false) where T : BaseStatus;

        /// <summary>
        /// Clear all statuses of a type
        /// </summary>
        /// <param name="type"></param>
        void ClearStatuses(Type type, bool forced = false);

        void ClearStatuses<T>(Actor source, bool forced = false) where T : BaseStatus;

        /// <summary>
        /// Clear all statuses caused by source id
        /// </summary>
        /// <param name="sourceId"></param>
        void ClearStatuses(Actor source, bool forced = false);

        /// <summary>
        /// Add all statuses caused by source id
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="statuses"></param>
        void AddStatuses(Actor source, GameObject[] statuses);

        /// <summary>
        /// Add a status caused by source 
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="statusPrefab"></param>
        BaseStatus AddStatus(Actor source, GameObject statusPrefab, bool forced = false);

        /// <summary>
        /// Add a status caused by source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="statusPrefab"></param>
        /// <param name="status"></param>
        /// <param name="forced"></param>
        /// <returns></returns>
        bool TryAddStatus(Actor source, GameObject statusPrefab, out BaseStatus status, bool forced = false);

        bool TryAddStatusDeferredStart(Actor source, GameObject statusPrefab, out BaseStatus status, bool forced = false);
    }
}