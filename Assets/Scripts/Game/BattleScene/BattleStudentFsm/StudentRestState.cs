using Game.Entity;
using Hali_Framework;

namespace Game.BattleScene.BattleStudentFsm
{
    public class StudentRestState : FsmState<BattleStudentEntity>
    {
        private BattleStudentEntity _student;
        
        protected internal override void OnEnter(IFsm<BattleStudentEntity> fsm)
        {
            base.OnEnter(fsm);
            _student = fsm.Owner;
            _student.IsControl = false;
            _student.useMove = false;
            _student.anim.SetBool(BattleStudentEntity.BattleMove, false);
            _student.anim.SetBool(BattleStudentEntity.BattleAim, false);
        }

        protected internal override void OnLeave(IFsm<BattleStudentEntity> fsm, bool isShutdown)
        {
            base.OnLeave(fsm, isShutdown);
            _student.IsControl = true;
            _student.useMove = true;
        }
    }
}