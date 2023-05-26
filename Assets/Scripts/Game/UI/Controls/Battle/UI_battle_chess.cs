using Game.BattleScene.BattleRole;
using Game.Entity;
using Hali_Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI.Controls.Battle
{
    public partial class UI_battle_chess : ControlBase
    {
        public RectTransform rotRect; 
        private IBattleRole _role;
        private Toggle _togChess;

        protected internal override void OnInit()
        {
            base.OnInit();
            _togChess = GetControl<Toggle>("tog_chess");
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            EventMgr.Instance.RemoveListener<bool, int>(ClientEvent.CHESS_AUTO_CLICK, OnChessAutoClick);
            EventMgr.Instance.RemoveListener<bool, int>(ClientEvent.CHESS_CLICK, OnOtherChessClick);
            EventMgr.Instance.RemoveListener<IBattleRole>(ClientEvent.BATTLE_ROLE_REST, OnRoleRest);
            UIMgr.RemoveCustomEventListener(this, EventTriggerType.PointerClick, OnClick);
        }

        public void SetData(IBattleRole role)
        {
            SetGray(role.RestOneRound);
            _role = role;
            EventMgr.Instance.AddListener<bool, int>(ClientEvent.CHESS_AUTO_CLICK, OnChessAutoClick);
            EventMgr.Instance.AddListener<bool, int>(ClientEvent.CHESS_CLICK, OnOtherChessClick);
            EventMgr.Instance.AddListener<IBattleRole>(ClientEvent.BATTLE_ROLE_REST, OnRoleRest);
            UIMgr.AddCustomEventListener(this, EventTriggerType.PointerClick, OnClick);
        }

        public void SetGray(bool isGray)
        {
            SetInteractable(!isGray);
            SetBlocksRaycasts(!isGray);
            _togChess.isOn = false;
        }

        private void OnRoleRest(IBattleRole role)
        {
            if (role.IsEnemy == _role.IsEnemy && role.RoleIndex == _role.RoleIndex)
                SetGray(true);
        }

        private void OnOtherChessClick(bool isEnemy, int roleIndex)
        {
            //如果是当前点击者，或者休息一回合的棋子，不用更新
            if(_role.IsEnemy == isEnemy && _role.RoleIndex == roleIndex || _role.RestOneRound) return;
            
            _togChess.isOn = false;
            SetInteractable(true);
            this.SetBlocksRaycasts(true);
        }

        private void OnChessAutoClick(bool isEnemy, int roleIndex)
        {
            if (_role.IsEnemy == isEnemy && _role.RoleIndex == roleIndex)
            {
                _togChess.isOn = true;
                EventMgr.Instance.TriggerEvent(ClientEvent.CHESS_CLICK, _role.IsEnemy, _role.RoleIndex);
                SetInteractable(false);
                this.SetBlocksRaycasts(false);
            }
        }

        private void OnClick(BaseEventData data)
        {
            _togChess.isOn = !_togChess.isOn;
            if (_togChess.isOn)
            {
                EventMgr.Instance.TriggerEvent(ClientEvent.CHESS_CLICK, _role.IsEnemy, _role.RoleIndex);
                SetInteractable(false);
                this.SetBlocksRaycasts(false);
            }
        }
    }
}