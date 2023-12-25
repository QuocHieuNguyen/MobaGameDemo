using System;
using Framework.Status;
using TypeReferences;
using UnityEngine;

namespace Framework
{
    [Serializable]
    public class StatusPoint
    {
        [SerializeField, ClassExtends(typeof(BaseStatus))]
        private ClassTypeReference _statusType;

        [SerializeField] private float _point;

        public float Point => _point;

        public ClassTypeReference StatusType => _statusType;
    }
}