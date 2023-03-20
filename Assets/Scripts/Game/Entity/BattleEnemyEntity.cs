using Game.BattleScene;
using Game.BattleScene.BattleEnemyFsm;
using Game.Model;
using Hali_Framework;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Entity
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class BattleEnemyEntity : BattleRoleEntity
    {
        public static readonly int BattleMove = Animator.StringToHash("move");
        public static readonly int BattleAim = Animator.StringToHash("aim");
        public static readonly int BattleShoot = Animator.StringToHash("shoot");
        public static readonly int BattleReload = Animator.StringToHash("reload");
        public static readonly int BattleDead = Animator.StringToHash("dead");
        
        private EnemyInfo _enemyInfo;
        private IFsm<BattleEnemyEntity> _fsm;
        private NavMeshAgent _agent;
        private Vector3 _targetPos;

        public EnemyInfo EnemyInfo => _enemyInfo;

        public NavMeshAgent Agent => _agent;

        public Vector3 TargetPos => _targetPos;

        public IFsm<BattleEnemyEntity> Fsm => _fsm;

        protected override void Awake()
        {
            base.Awake();
            IsControl = false;
            _agent = GetComponent<NavMeshAgent>();
            gameObject.tag = GameConst.ENEMY_TAG;
        }

        protected override void Update()
        {
        }

        public void SetEnemyInfo(EnemyInfo info)
        {
            _enemyInfo = info;
            maxHp = curHp = _enemyInfo.hp;
            maxAp = curAp = _enemyInfo.ap;
            maxAmmo = curAmmo = _enemyInfo.ammo;
            atkType = (AtkType)info.atkType;
            atkRange = info.atkRange;
        }

        public void SetMovePos(Vector3 target) => _targetPos = target;

        public override void InitFsm(int index)
        {
            isEnemy = true;
            roleIndex = index;
            _fsm = FsmMgr.Instance.CreateFsm($"{BattleConst.BATLLE_ROLE_FSM}_Enemy_{index}", this,
                new EnemyRestState(), new EnemyMoveState(), new EnemyAimState(), new EnemyDeadState());
            _fsm.Start<EnemyRestState>();
        }

        protected override void OnSwitchRole(BattleRoleEntity role)
        {
            if(role == this)
                _fsm.ChangeState<EnemyMoveState>();
            else
                _fsm.ChangeState<EnemyRestState>();
        }

        public override void SubHp(int hp)
        {
            base.SubHp(hp);
            if (isDead)
                anim.SetBool(BattleDead, true);
        }
    }
}