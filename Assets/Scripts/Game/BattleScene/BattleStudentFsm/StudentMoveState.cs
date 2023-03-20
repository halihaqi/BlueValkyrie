using Game.Entity;
using Hali_Framework;
using UnityEngine;

namespace Game.BattleScene.BattleStudentFsm
{
    public class StudentMoveState : FsmState<BattleStudentEntity>
    {
        private BattleStudentEntity _role;
        private IFsm<BattleStudentEntity> _fsm;
        private BattleMaster _bm;
        private FlagEntity _aroundFlag;
        
        protected internal override void OnEnter(IFsm<BattleStudentEntity> fsm)
        {
            base.OnEnter(fsm);
            InputMgr.Instance.Enabled = true;
            _role = fsm.Owner;
            _bm = FsmMgr.Instance.GetFsm<BattleMaster>(BattleConst.BATTLE_FSM).Owner;
            _fsm = fsm;
            var bm = FsmMgr.Instance.GetFsm<BattleMaster>(BattleConst.BATTLE_FSM).Owner;
            _role.SetFollowCamera(bm.cam, bm.followCam);
            _role.useMove = true;
            EventMgr.Instance.AddListener<KeyCode>(ClientEvent.GET_KEY_DOWN, OnKeyDown);
        }

        protected internal override void OnUpdate(IFsm<BattleStudentEntity> fsm, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(fsm, elapseSeconds, realElapseSeconds);
            _role.anim.SetBool(BattleStudentEntity.BattleMove, _role.IsMove);
            if (_bm.IsRoleAroundFlag(_role, out var flag))
            {
                _bm.BattlePanel.SetFlagTipActive(true);
                _aroundFlag = flag;
            }
            else
            {
                _bm.BattlePanel.SetFlagTipActive(false);
                _aroundFlag = null;
            }
            
            if (_role.CurAp <= 0)
            {
                _role.useMove = false;
                return;
            }
            _role.SubAp(_role.cc.velocity.magnitude * 0.02f);
        }

        protected internal override void OnLeave(IFsm<BattleStudentEntity> fsm, bool isShutdown)
        {
            base.OnLeave(fsm, isShutdown);
            _role.anim.SetBool(BattleStudentEntity.BattleMove, false);
            EventMgr.Instance.RemoveListener<KeyCode>(ClientEvent.GET_KEY_DOWN, OnKeyDown);
        }

        private void OnKeyDown(KeyCode keyCode)
        {
            //切换Aim状态
            if (keyCode == KeyCode.R)
            {
                ChangeState<StudentAimState>(_fsm);
                EventMgr.Instance.TriggerEvent(ClientEvent.BATTLE_ROLE_AIM);
            }

            //结束回合
            if (keyCode == KeyCode.Q)
            {
                ChangeState<StudentRestState>(_fsm);
                EventMgr.Instance.TriggerEvent(ClientEvent.BATTLE_STEP_OVER);
            }
            
            //夺旗
            if (keyCode == KeyCode.F)
            {
                if(_aroundFlag == null) return;
                
                ChangeState<StudentRestState>(_fsm);
                _bm.RiseFlag(_aroundFlag, FlagType.Student);
            }
        }
    }
}