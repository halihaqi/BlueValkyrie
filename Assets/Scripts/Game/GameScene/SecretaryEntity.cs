using System;
using Game.Base;
using Game.Managers;
using UnityEngine;

namespace Game.GameScene
{
    public class SecretaryEntity : AgentRoleBase
    {
        private float _reactionInterval = 6f;
        private float _reactionElapseSeconds;
        
        private static readonly int Speed = Animator.StringToHash("speed");
        private static readonly int Reaction = Animator.StringToHash("reaction");
        public Transform FollowTarget { get; set; }

        protected override void Awake()
        {
            base.Awake();
            SetAutoBraking(true);
            MoveSpeed = 3.5f;
            
            UsePush = true;
        }

        private void Start()
        {
            SetPosition(GameSceneMonoMgr.Instance.playerBornPos - Vector3.forward * 2);
            _reactionElapseSeconds = 0;
        }

        private void Update()
        {
            var velocity = agent.velocity.magnitude;
            _reactionElapseSeconds = velocity < 0.01f ? _reactionElapseSeconds + Time.deltaTime : 0;

            if (!isPush && FollowTarget != null)
            {
                agent.SetDestination(FollowTarget.position);
            }
            anim.SetFloat(Speed, velocity);
            
            if (_reactionElapseSeconds > _reactionInterval)
            {
                _reactionElapseSeconds = 0;
                anim.SetTrigger(Reaction);
            }
        }

        public void SetPosition(Vector3 pos)
        {
            agent.SetDestination(pos);
            agent.Warp(pos);
        }

        public void SetFollowDistance(float distance) => StopDistance = distance;
    }
}