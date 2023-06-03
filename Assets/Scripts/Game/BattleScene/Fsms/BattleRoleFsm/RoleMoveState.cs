using Game.BattleScene.BattleRole;
using Hali_Framework;
using UnityEngine;

namespace Game.BattleScene.Fsms.BattleRoleFsm
{
    public class RoleMoveState : FsmState<IBattleRole>
    {
        private Vector3 _lastPosition;
        private IFsm<IBattleRole> _myFsm;
        private IBattleRole _role;
        private const float SUB_AP_DIS = 0.3f;
        private const float SUB_AP_FACTOR = 1f;
        
        protected internal override void OnEnter(IFsm<IBattleRole> fsm)
        {
            base.OnEnter(fsm);
            _role = fsm.Owner;
            _myFsm = fsm;
            //恢复操控
            _role.IsControl = true;
            InputMgr.Instance.Enabled = true;
            EventMgr.Instance.AddListener<KeyCode>(ClientEvent.GET_KEY, OnMove);
            EventMgr.Instance.AddListener<KeyCode>(ClientEvent.GET_KEY_DOWN, OnKeyDown);
            _lastPosition = _role.Go.transform.position;
        }

        protected internal override void OnUpdate(IFsm<IBattleRole> fsm, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(fsm, elapseSeconds, realElapseSeconds);
            if (_role.CurAp <= 0)
            {
                _role.IsControl = false;
                return;
            }

            float dis = Vector3.Distance(_role.Go.transform.position, _lastPosition);
            //阈值以内不减体力
            if(dis < SUB_AP_DIS) return;
            
            _lastPosition = _role.Go.transform.position;
            _role.SubAp(dis * SUB_AP_FACTOR);
        }

        protected internal override void OnLeave(IFsm<IBattleRole> fsm, bool isShutdown)
        {
            base.OnLeave(fsm, isShutdown);
            _role = null;
            _myFsm = null;
            EventMgr.Instance.RemoveListener<KeyCode>(ClientEvent.GET_KEY, OnMove);
            EventMgr.Instance.RemoveListener<KeyCode>(ClientEvent.GET_KEY_DOWN, OnKeyDown);
        }

        private void OnMove(KeyCode key)
        {
            
        }
        
        private void OnKeyDown(KeyCode keyCode)
        {
            //切换Aim状态
            if (keyCode == KeyCode.R)
            {
                ChangeState<RoleAimState>(_myFsm);
                //EventMgr.Instance.TriggerEvent(ClientEvent.BATTLE_ROLE_AIM);
            }

            //结束回合
            else if (keyCode == KeyCode.Q)
            {
                ChangeState<RoleResetState>(_myFsm);
                //EventMgr.Instance.TriggerEvent(ClientEvent.BATTLE_STEP_OVER);
            }
            
            //夺旗
            else if (keyCode == KeyCode.F)
            {
                // if(_aroundFlag == null) return;
                //
                // ChangeState<StudentRestState>(_fsm);
                // _bm.RiseFlag(_aroundFlag, FlagType.Student);
            }
        }
    }
}