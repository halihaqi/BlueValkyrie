using System;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Base
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AgentRoleBase : RoleBase
    {
        protected NavMeshAgent agent;

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
    }
}