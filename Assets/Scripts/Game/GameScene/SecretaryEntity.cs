using System;
using Game.Base;
using Game.Managers;
using UnityEngine;

namespace Game.GameScene
{
    public class SecretaryEntity : AgentRoleBase
    {
        private static readonly int Speed = Animator.StringToHash("speed");
        public Transform FollowTarget { get; set; }

        protected override void Awake()
        {
            base.Awake();
            SetAutoBraking(true);
            MoveSpeed = 3.5f;
        }

        private void Start()
        {
            SetPosition(GameSceneMonoMgr.Instance.playerBornPos - Vector3.forward * 2);
        }

        private void Update()
        {
            if (FollowTarget != null)
                agent.SetDestination(FollowTarget.position);
            anim.SetFloat(Speed, agent.velocity.magnitude);
        }

        public void SetPosition(Vector3 pos)
        {
            agent.SetDestination(pos);
            agent.Warp(pos);
        }

        public void SetFollowDistance(float distance) => StopDistance = distance;
    }
}