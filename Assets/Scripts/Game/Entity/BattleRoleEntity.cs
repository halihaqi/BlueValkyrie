using System;
using Hali_Framework;
using UnityEngine;

namespace Game.Entity
{
    public class BattleRoleEntity : ThirdPersonController
    {
        private BattleRoleInfo _info;
        private float _maxAp;
        private float _curAp;
        private bool _isControl = false;
        private static readonly int BattleMove = Animator.StringToHash("battle_move");
        private static readonly int BattleAim = Animator.StringToHash("battle_anim");
        private static readonly int BattleShoot = Animator.StringToHash("battle_shoot");
        private static readonly int BattleReload = Animator.StringToHash("battle_reload");
        private static readonly int BattleDead = Animator.StringToHash("battle_dead");

        private const string SHELTER_TAG = "Shelter";

        public bool IsControl
        {
            get => _isControl;
            set
            {
                IsMove = _isControl = value;
                anim.SetBool(BattleMove, IsMove);
            }
        }
        
        protected override void Awake()
        {
            base.Awake();
            useJump = false;
            useSprint = false;
            useAnim = false;

            followTarget = transform.Find("followTarget");
            gameObject.tag = "Role";
            cc.center = Vector3.up * 1.03f;
        }

        protected override void Start()
        {
            //先开启正常战斗模式
            anim.SetLayerWeight(2, 1);
        }

        protected override void OnDestroy()
        {
        }

        protected override void Update()
        {
            if (!_isControl || followCamera == null) return;
            if (_curAp <= 0)
            {
                IsControl = false;
                return;
            }
            base.Update();
            anim.SetBool(BattleMove, IsMove);
            _curAp -= cc.velocity.magnitude * 0.005f;
            EventMgr.Instance.TriggerEvent(ClientEvent.BATTLE_ROLE_MOVE, _curAp);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(SHELTER_TAG))
            {
                SwitchBattleMode(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(SHELTER_TAG))
            {
                SwitchBattleMode(false);
            }
        }

        public void SetBattleRoleInfo(BattleRoleInfo info)
        {
            _info = info;
            _curAp = _maxAp = _info.baseAp;
        }

        public void SetFollowCamera(Camera cam)
        {
            followCamera = cam;

            if(cam == null) return;
            if (!followCamera.TryGetComponent(out thirdPersonCam))
                thirdPersonCam = followCamera.gameObject.AddComponent<ThirdPersonCam>();
            thirdPersonCam.followTarget = followTarget;
        }

        public void ResetAp()
        {
            _curAp = _maxAp;
        }

        public void SwitchBattleMode(bool isShelt)
        {
            anim.SetLayerWeight(2, isShelt ? 0 : 1);
            anim.SetLayerWeight(3, isShelt ? 1 : 0);
        }
    }
}