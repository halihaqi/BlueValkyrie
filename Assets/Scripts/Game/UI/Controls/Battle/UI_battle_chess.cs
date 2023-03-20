using Game.Entity;
using Hali_Framework;
using UnityEngine.UI;

namespace Game.UI.Controls.Battle
{
    public class UI_battle_chess : ControlBase
    {
        private BattleRoleEntity _role;
        private Toggle _togChess;

        protected internal override void OnInit()
        {
            base.OnInit();
            _togChess = GetControl<Toggle>("tog_chess");
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            _togChess.onValueChanged.RemoveAllListeners();
            EventMgr.Instance.RemoveListener<bool, int>(ClientEvent.CHESS_AUTO_CLICK, OnChessAutoClick);
            EventMgr.Instance.RemoveListener<bool, int>(ClientEvent.CHESS_CLICK, OnChessClick);
            EventMgr.Instance.RemoveListener<BattleRoleEntity>(ClientEvent.BATTLE_ROLE_REST, OnRoleRest);
        }

        public void SetRole(BattleRoleEntity role)
        {
            SetGray(role.RestOneRound);
            _role = role;
            _togChess.onValueChanged.RemoveListener(OnToggle);
            _togChess.onValueChanged.AddListener(OnToggle);
            EventMgr.Instance.AddListener<bool, int>(ClientEvent.CHESS_AUTO_CLICK, OnChessAutoClick);
            EventMgr.Instance.AddListener<bool, int>(ClientEvent.CHESS_CLICK, OnChessClick);
            EventMgr.Instance.AddListener<BattleRoleEntity>(ClientEvent.BATTLE_ROLE_REST, OnRoleRest);
        }

        public void SetGray(bool isGray)
        {
            _togChess.interactable = !isGray;
            this.SetBlocksRaycasts(!isGray);
            _togChess.isOn = false;
        }

        private void OnRoleRest(BattleRoleEntity role)
        {
            if (role.IsEnemy == _role.IsEnemy && role.RoleIndex == _role.RoleIndex)
                SetGray(true);
        }

        private void OnChessClick(bool isEnemy, int roleIndex)
        {
            //如果是当前点击者，或者休息一回合的棋子，不用更新
            if(_role.IsEnemy == isEnemy && _role.RoleIndex == roleIndex || _role.RestOneRound) return;
            
            _togChess.isOn = false;
            _togChess.interactable = true;
            this.SetBlocksRaycasts(true);
        }

        private void OnChessAutoClick(bool isEnemy, int roleIndex)
        {
            if (_role.IsEnemy == isEnemy && _role.RoleIndex == roleIndex)
                _togChess.isOn = true;
        }

        private void OnToggle(bool isOn)
        {
            if (isOn)
            {
                EventMgr.Instance.TriggerEvent(ClientEvent.CHESS_CLICK, _role.IsEnemy, _role.RoleIndex);
                _togChess.interactable = false;
                this.SetBlocksRaycasts(false);
            }
        }
    }
}