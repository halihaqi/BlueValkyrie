using System;
using Hali_Framework;
using UnityEngine;

namespace Game.Base
{
    [RequireComponent(typeof(CapsuleCollider))]
    public abstract class RoleBase : MonoBehaviour
    {
        private RoleInfo _roleInfo;
        
        [SerializeField]
        protected float moveSpeed = 1.5f;
        [SerializeField]
        protected float rotateSpeed = 10;
        
        protected Animator _anim;
        protected CapsuleCollider _collider;

        public RoleInfo RoleInfo => _roleInfo;

        public string ResPath => _roleInfo == null ? null : $"Prefabs/Students/{_roleInfo.school}/{_roleInfo.name}";

        protected virtual void Awake()
        {
            _anim = GetComponentInChildren<Animator>();
            _collider = GetComponent<CapsuleCollider>();

            if (_anim == null || _collider == null)
                throw new Exception("Role Prefab has no animator or collider.");
            SetTrigger(false);
            SetColliderSize(Vector3.up * 1.08f, 0.5f, 2f);
            
            gameObject.layer = LayerMask.NameToLayer(GameConst.ROLE_LAYER);
        }

        public void SetRoleInfo(RoleInfo info) => _roleInfo = info;

        protected void SetColliderSize(Vector3 center, float radius, float height)
        {
            _collider.center = center;
            _collider.radius = radius;
            _collider.height = height;
        }

        protected void SetTrigger(bool isTrigger)
        {
            _collider.isTrigger = isTrigger;
        }
    }
}