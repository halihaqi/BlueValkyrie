using System;
using Game.Managers;
using Game.UI.Game;
using Hali_Framework;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public partial class UI_equip_item : ControlBase
    {
        private UI_normal_item _normalItem;
        private Text _txtAttri;
        private Text _txtAttriNum;
        private Image _imgFull;
        private Image _imgEmpty;

        private Action _clickEvent;

        protected internal override void OnInit()
        {
            base.OnInit();
            _normalItem = GetControl<UI_normal_item>("normal_item");
            _txtAttri = GetControl<Text>("txt_attri");
            _txtAttriNum = GetControl<Text>("txt_attri_num");
            _imgFull = GetControl<Image>("img_full");
            _imgEmpty = GetControl<Image>("img_empty");
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            UIMgr.RemoveCustomEvent(_normalItem, EventTriggerType.PointerClick);
            UIMgr.RemoveCustomEvent(_imgEmpty, EventTriggerType.PointerClick);
        }

        public void SetData(EquipInfo info)
        {
            UIMgr.AddCustomEventListener(_normalItem, EventTriggerType.PointerClick, OnClick);
            UIMgr.AddCustomEventListener(_imgEmpty, EventTriggerType.PointerClick, OnClick);
            _imgFull.gameObject.SetActive(info != null);
            _imgEmpty.gameObject.SetActive(info == null);

            if (info == null) return;
            
            _normalItem.SetData(info.itemId);
            _txtAttri.text = EquipMgr.GetAttributeName(info.type);
            _txtAttriNum.text = info.attribute.ToString();
        }

        private void OnClick(BaseEventData data)
        {
            _clickEvent?.Invoke();
        }

        public void SetClickListener(Action callback)
        {
            _clickEvent = callback;
        }
    }
}