using System;
using Hali_Framework;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Base
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AgentRoleBase : RoleBase
    {
        protected NavMeshAgent agent;
        [SerializeField]
        private float mass = 1;

        protected bool isPush;
        private bool _usePush = false;
        private CapsuleCollider _pushTrigger;

        public bool UsePush
        {
            get => _usePush;
            set
            {
                if(_usePush == value) return;
                _usePush = value;
                if (_usePush)
                {
                    _pushTrigger ??= gameObject.AddComponent<CapsuleCollider>();
                    _pushTrigger.enabled = true;
                    _pushTrigger.isTrigger = true;
                    _pushTrigger.height = roleCollider.height;
                    _pushTrigger.radius = roleCollider.radius + 0.25f;
                    _pushTrigger.center = roleCollider.center;
                }
                else
                {
                    _pushTrigger.enabled = false;
                }
            }
        }

        public float MoveSpeed
        {
            get => moveSpeed;
            protected set
            {
                if(Math.Abs(moveSpeed - value) < 0.01f) return;
                moveSpeed = value;
                agent.speed = moveSpeed;
            }
        }
        
        public float AngularSpeed
        {
            get => angularSpeed;
            protected set
            {
                if(Math.Abs(angularSpeed - value) < 0.01f) return;
                angularSpeed = value;
                agent.angularSpeed = angularSpeed;
            }
        }
        
        public float StopDistance
        {
            get => stopDistance;
            protected set
            {
                if(Math.Abs(stopDistance - value) < 0.01f) return;
                stopDistance = value;
                agent.stoppingDistance = stopDistance;
            }
        }

        protected override void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            base.Awake();

            agent.autoBraking = false;
            agent.speed = moveSpeed;
            agent.angularSpeed = angularSpeed;
            agent.stoppingDistance = stopDistance;
        }

        protected override void SetColliderSize(Vector3 center, float radius, float height)
        {
            base.SetColliderSize(center, radius, height);
            agent.height = height;
            agent.radius = radius;
        }
        
        
        protected void SetAutoBraking(bool isAutoBraking) => agent.autoBraking = isAutoBraking;

        private void OnTriggerStay(Collider other)
        {
            if (_usePush && other.CompareTag(GameConst.PLAYER_TAG))
            {
                isPush = true;
                Push(transform.position - other.transform.position, mass);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            isPush = false;
        }

        public void Push(Vector3 direction, float force)
        {
            if (!_usePush) return;
            // 计算推动的力量
            Vector3 pushForce = direction.normalized * force;
            // 停止当前的导航行动
            agent.ResetPath();
            // 添加推力
            agent.velocity += pushForce;
        }
    }
}