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
        private AtkType _atkType;
        private RoleType _roleType;
        
        private int _curHp;
        private float _curAp;
        private int _curAmmo;
        private bool _isDead;
        private BattleRoleState _roleState;

        //角色信息
        public int RoleId => _roleId;
        public AtkType AtkType => _atkType;
        public RoleType RoleType => _roleType;
        public GameObject Go => gameObject;

        //角色状态
        public bool IsControl { get; set; }
        public int CurHp => _curHp;
        public float CurAp => _curAp;
        public int CurAmmo => _curAmmo;
        public bool IsDead => _isDead;
        public IBattleRole AtkTarget { get; set; }
        public BattleRoleState RoleState => _roleState;

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
            _curAmmo = _roleState.maxAmmo;
        }

        public void InitMe(object info)
        {
            if (!(info is StudentItem student))
                throw new Exception("Init student info valid.");
            _roleId = student.roleId;
            _atkType = student.AtkType;
            _roleType = RoleType.Student;
            BattleRoleState state = new BattleRoleState
            {
                maxHp = student.Hp,
                maxAp = student.Ap,
                maxAmmo = student.Ammo,
                atkRange = student.atkRange,
                atk = student.Atk,
                def = student.Def,
                //todo 暴击和闪避
            };
            
            _curHp = student.Hp;
            _curAp = student.Ap;
            _curAmmo = student.Ammo;
            _roleState = state;

            _isDead = false;
            IsControl = false;
            AtkTarget = null;
            anim.SetBool(BattleDead, false);
        }

        private void KillMe()
        {
            _isDead = true;
            IsControl = false;
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