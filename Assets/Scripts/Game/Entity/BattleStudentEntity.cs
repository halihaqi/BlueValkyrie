using Game.BattleScene;
using Game.BattleScene.BattleStudentFsm;
using Game.Managers;
using Game.Model;
using Hali_Framework;
using UnityEngine;

namespace Game.Entity
{
    public class BattleStudentEntity : BattleRoleEntity
    {
        public static readonly int BattleMove = Animator.StringToHash("battle_move");
        public static readonly int BattleAim = Animator.StringToHash("battle_aim");
        public static readonly int BattleShoot = Animator.StringToHash("battle_shoot");
        public static readonly int BattleReload = Animator.StringToHash("battle_reload");
        public static readonly int BattleDead = Animator.StringToHash("battle_dead");
        
        private StudentItem _student;
        private IFsm<BattleStudentEntity> _fsm;

        public StudentItem Student => _student;

        public IFsm<BattleStudentEntity> Fsm => _fsm;

        protected override void Start()
        {
            base.Start();
            //先开启正常战斗模式
            anim.SetLayerWeight(2, 1);
            gameObject.tag = GameConst.STUDENT_TAG;
        }

        protected override void Update()
        {
            if (!IsControl || followCamera == null) return;
            base.Update();
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

        public override void InitFsm(int index)
        {
            isEnemy = false;
            roleIndex = index;
            _fsm = FsmMgr.Instance.CreateFsm($"{BattleConst.BATLLE_ROLE_FSM}_Student_{index}", this,
                new StudentRestState(), new StudentMoveState(), new StudentAimState(), new StudentDeadState());
            _fsm.Start<StudentRestState>();
        }

        protected override void OnSwitchRole(BattleRoleEntity role)
        {
            if(role == this)
                _fsm.ChangeState<StudentMoveState>();
            else
                _fsm.ChangeState<StudentRestState>();
        }

        public override void SubHp(int hp)
        {
            base.SubHp(hp);
            if (isDead)
                anim.SetBool(BattleDead, true);
        }

        public void SetStudentInfo(StudentItem student)
        {
            _student = student;
            maxHp = curHp = student.Hp;
            maxAp = curAp = student.Ap;
            maxAmmo = curAmmo = student.Ammo;
            atkType = (AtkType)student.atkType;
            atkRange = student.atkRange;
        }
    }
}