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
        private Action _clickEvent;

        protected internal override void OnInit()
        {
            base.OnInit();
            normal_item = GetControl<UI_normal_item>("normal_item");
            txt_attri = GetControl<Text>("txt_attri");
            txt_attri_num = GetControl<Text>("txt_attri_num");
            img_full = GetControl<Image>("img_full");
            img_empty = GetControl<Image>("img_empty");
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            UIMgr.RemoveCustomEvent(normal_item, EventTriggerType.PointerClick);
            UIMgr.RemoveCustomEvent(img_empty, EventTriggerType.PointerClick);
        }

        public void SetData(EquipInfo info)
        {
            UIMgr.AddCustomEventListener(normal_item, EventTriggerType.PointerClick, OnClick);
            UIMgr.AddCustomEventListener(img_empty, EventTriggerType.PointerClick, OnClick);
            img_full.gameObject.SetActive(info != null);
            img_empty.gameObject.SetActive(info == null);

            if (info == null) return;
            
            normal_item.SetData(info.itemId);
            txt_attri.text = EquipMgr.GetAttributeName(info.type);
            txt_attri_num.text = info.attribute.ToString();
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