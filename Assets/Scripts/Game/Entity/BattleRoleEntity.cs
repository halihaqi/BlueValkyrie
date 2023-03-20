using System.Collections.Generic;
using Game.BattleScene;
using Game.Model;
using Hali_Framework;
using UnityEngine;

namespace Game.Entity
{
    public abstract class BattleRoleEntity : ThirdPersonController
    {
        public const float ORI_CAM_DIS = 2f;
        public const float ORI_CAM_SENSITIVE = 12f;
        public const float ORI_CAM_OFFSET_X = 1f; 
        public const float AIM_CAM_DIS = 1.3f;
        public const float AIM_CAM_SENSITIVE = 4f;
        public const float AIM_CAM_OFFSET_X = 2f;

        private bool _isControl = false;
        private bool _restOneRound = false;
        private FireEntity _fireEntity;
        private BattleRoleEntity _target;

        public Transform modelTrans;

        protected int curHp;
        protected float curAp;
        protected int curAmmo;
        protected bool isDead = false;

        protected bool isEnemy;
        protected int roleIndex;
        protected int maxHp;
        protected float maxAp;
        protected int maxAmmo;
        protected AtkType atkType;
        protected int atkRange;
        

        public bool IsControl
        {
            get => _isControl;
            set => _isControl = value;
        }

        public int CurHp => curHp;

        public float CurAp => curAp;

        public int CurAmmo => curAmmo;

        public int MaxHp => maxHp;

        public float MaxAp => maxAp;

        public int MaxAmmo => maxAmmo;

        public AtkType AtkType => atkType;

        public int AtkRange => atkRange;
        
        public bool IsEnemy => isEnemy;

        public int RoleIndex => roleIndex;

        public bool IsDead => isDead;

        public bool RestOneRound => _restOneRound;

        public FireEntity FireEntity => _fireEntity;

        public BattleRoleEntity Target
        {
            get => _target;
            set
            {
                if(_target == value) return;
                _target = value;
                FireEntity.SetTarget(_target != null ? _target.followTarget : null);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            useJump = false;
            useSprint = false;
            useAnim = false;
            followTarget = transform.Find("followTarget");
            modelTrans = anim.gameObject.transform;
            cc.center = Vector3.up * 1.03f;
            gameObject.layer = LayerMask.NameToLayer(GameConst.ROLE_LAYER);
            if (anim.gameObject.TryGetComponent(out FireEntity fireEntity))
            {
                _fireEntity = fireEntity;
                _fireEntity.SetOwner(this);
            }
            EventMgr.Instance.AddListener<BattleRoleEntity>(ClientEvent.BATTLE_ROLE_CHANGE, OnSwitchRole);
            EventMgr.Instance.AddListener(ClientEvent.BATTLE_ROUND_OVER, OnRoundOver);
        }

        protected override void Start()
        {
        }

        protected override void Update()
        {
            if (isDead || !_isControl || followCamera == null) return;
            base.Update();
        }

        protected override void OnDestroy()
        {
            EventMgr.Instance.RemoveListener<BattleRoleEntity>(ClientEvent.BATTLE_ROLE_CHANGE, OnSwitchRole);
            EventMgr.Instance.RemoveListener(ClientEvent.BATTLE_ROUND_OVER, OnRoundOver);
        }

        public void SetFollowCamera(Camera cam, ThirdPersonCam followCam)
        {
            followCamera = cam;
            thirdPersonCam = followCam;
            thirdPersonCam.followTarget = followTarget;
        }

        public void SubAp(float ap)
        {
            if(isDead) return;
            curAp -= ap;
        }

        public virtual void SubHp(int hp)
        {
            if(isDead) return;
            curHp -= hp;
            if (curHp > 0) return;

            isDead = true;
            _isControl = false;
            _restOneRound = false;
            curHp = 0;
            curAp = 0;
            curAmmo = 0;
            EventMgr.Instance.RemoveListener<BattleRoleEntity>(ClientEvent.BATTLE_ROLE_CHANGE, OnSwitchRole);
            EventMgr.Instance.RemoveListener(ClientEvent.BATTLE_ROUND_OVER, OnRoundOver);
        }

        public void SubAmmo()
        {
            if(isDead) return;
            --curAmmo;
            EventMgr.Instance.TriggerEvent(ClientEvent.BATTLE_ROLE_RELOAD);
        }

        private void OnRoundOver()
        {
            if(isDead) return;
            curAp = maxAp;
            _restOneRound = false;
        }

        public void ResetAmmo()
        {
            if(isDead) return;
            curAmmo = maxAmmo;
            _restOneRound = true;
        }

        public abstract void InitFsm(int index);

        protected abstract void OnSwitchRole(BattleRoleEntity role);
    }
}