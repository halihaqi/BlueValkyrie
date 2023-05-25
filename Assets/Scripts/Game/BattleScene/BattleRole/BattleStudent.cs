using System;
using Game.Model;
using Hali_Framework;
using UnityEngine;

namespace Game.BattleScene.BattleRole
{
    public class BattleStudent : ThirdPersonController, IBattleRole
    {
        public static readonly int BattleMove = Animator.StringToHash("battle_move");
        public static readonly int BattleAim = Animator.StringToHash("battle_aim");
        public static readonly int BattleShoot = Animator.StringToHash("battle_shoot");
        public static readonly int BattleReload = Animator.StringToHash("battle_reload");
        public static readonly int BattleDead = Animator.StringToHash("battle_dead");
        
        private int _roleId;
        private int _curHp;
        private float _curAp;
        private int _curAmmo;
        private int _maxHp;
        private float _maxAp;
        private int _maxAmmo;
        private bool _isControl;
        private bool _isDead;
        private RoleType _roleType;
        private AtkType _atkType;
        private int _atkRange;
        
        //角色信息
        public int RoleId => _roleId;
        public int MaxHp => _maxHp;
        public float MaxAp => _maxAp;
        public int MaxAmmo => _maxAmmo;
        public AtkType AtkType => _atkType;
        public int AtkRange => _atkRange;
        public RoleType RoleType => _roleType;
        public GameObject Go => gameObject;

        //角色状态
        public bool IsControl
        {
            get => _isControl;
            set => _isControl = value;
        }
        public int CurHp => _curHp;
        public float CurAp => _curAp;
        public int CurAmmo => _curAmmo;
        public bool IsDead => _isDead;
        public IBattleRole AtkTarget { get; set; }

        public void SubAp(float ap)
        {
            if(_isDead) return;
            _curAp -= ap;
        }

        public void SubHp(int hp)
        {
            if(_isDead) return;
            _curHp -= hp;
            if (_curHp > 0) return;
            KillMe();
        }

        public void SubAmmo(int num = 1)
        {
            if(_isDead) return;
            _curAmmo -= num;
        }

        public void ResetAmmo()
        {
            if(_isDead) return;
            _curAmmo = _maxAmmo;
        }

        public void InitMe(object info)
        {
            if (!(info is StudentItem student))
                throw new Exception("Init student info valid.");
            _roleId = student.roleId;
            _maxHp = _curHp = student.Hp;
            _maxAp = _curAp = student.Ap;
            _maxAmmo = _curAmmo = student.Ammo;
            _atkType = student.AtkType;
            _atkRange = student.atkRange;
            _roleType = RoleType.Student;

            _isDead = false;
            _isControl = false;
            AtkTarget = null;
            anim.SetBool(BattleDead, false);
        }

        private void KillMe()
        {
            _isDead = true;
            _isControl = false;
            _curHp = 0;
            _curAp = 0;
            _curAmmo = 0;
            anim.SetBool(BattleDead, true);
        }

        #region 生命周期

        protected override void Awake()
        {
            base.Awake();
            useJump = false;
            useSprint = false;
            useAnim = false;
            followTarget = transform.Find("followTarget");
            cc.center = Vector3.up * 1.03f;
            gameObject.layer = LayerMask.NameToLayer(GameConst.ROLE_LAYER);
        }

        protected override void Start()
        {
            anim.SetLayerWeight(2, 1);
            gameObject.tag = GameConst.STUDENT_TAG;
        }

        protected override void Update()
        {
            if (!IsControl || followCamera == null) return;
            base.Update();
        }

        protected override void OnDestroy()
        {
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(BattleConst.SHELTER_TAG))
                SwitchAnimMode(true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(BattleConst.SHELTER_TAG))
                SwitchAnimMode(false);
        }
        
        private void SwitchAnimMode(bool isShelter)
        {
            anim.SetLayerWeight(2, isShelter ? 0 : 1);
            anim.SetLayerWeight(3, isShelter ? 1 : 0);
        }

        #endregion
    }
}