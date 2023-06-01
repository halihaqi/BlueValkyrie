using Game.BattleScene.BattleRole;
using Game.UI.Base;
using Game.Utils;
using Hali_Framework;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI.Controls
{
    public partial class UI_role_chess : ControlBase
    {
        private IBattleRole _role;
        private ControlGroup _ring;
        private bool _selected = false;
        public bool Selected => _selected;

        protected internal override void OnInit()
        {
            base.OnInit();
            UIMgr.AddCustomEventListener(this, EventTriggerType.PointerClick, OnClick);
            _ring = img_ring.GetComponent<ControlGroup>();
            EventMgr.Instance.AddListener<IBattleRole>(ClientEvent.CHESS_CLICK, OnChessClick);
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            UIMgr.RemoveAllCustomEvents(this);
            EventMgr.Instance.RemoveListener<IBattleRole>(ClientEvent.CHESS_CLICK, OnChessClick);
        }

        public void SetData(IBattleRole role)
        {
            _role = role;
            ResMgr.Instance.LoadAsync<Sprite>(GameConst.RES_GROUP_UI, 
                ResPath.GetChessIcon(role.RoleType), img =>
            {
                img_chess.sprite = img;
            });
            _ring.SetAlpha(0);
        }

        private void OnClick(BaseEventData data)
        {
            EventMgr.Instance.TriggerEvent(ClientEvent.CHESS_CLICK, _role);
        }

        private void OnChessClick(IBattleRole role)
        {
            _selected = role == _role;
            _ring.SetAlpha(_selected ? 1 : 0);
            SetBlocksRaycasts(!_selected);
        }
    }
}