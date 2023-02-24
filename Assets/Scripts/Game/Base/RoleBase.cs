using System;
using Hali_Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Game.Base
{
    [RequireComponent(typeof(CapsuleCollider))]
    public abstract class RoleBase : MonoBehaviour
    {
        private RoleInfo _roleInfo;

        [SerializeField]
        protected float moveSpeed = 3.5f;
        [SerializeField]
        protected float angularSpeed = 120;
        [SerializeField]
        protected float stopDistance = 0.15f;
        
        protected Animator anim;
        protected CapsuleCollider roleCollider;

        public RoleInfo RoleInfo => _roleInfo;

        public string ResPath => _roleInfo == null ? null : Utils.ResPath.GetStudentObjPath(_roleInfo);

        protected virtual void Awake()
        {
            anim = GetComponentInChildren<Animator>();
            roleCollider = GetComponent<CapsuleCollider>();

            if (anim == null)
                throw new Exception("Role Prefab has no animator.");
            SetTrigger(false);
            SetColliderSize(Vector3.up * 1.08f, 0.5f, 2f);

            gameObject.layer = LayerMask.NameToLayer(GameConst.ROLE_LAYER);
        }

        public void SetRoleInfo(RoleInfo info) => _roleInfo = info;

        protected virtual void SetColliderSize(Vector3 center, float radius, float height)
        {
            roleCollider.center = center;
            roleCollider.radius = radius;
            roleCollider.height = height;
        }

        protected void SetTrigger(bool isTrigger)
        {
            roleCollider.isTrigger = isTrigger;
        }
    }
}