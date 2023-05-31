using Game.Managers;
using Game.Model.BagModel;
using Hali_Framework;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public partial class UI_lv_item : ControlBase
    {
        private int _num = 0;
        private int _bagNum;
        private int _itemId;

        private int _preLv;

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            UIMgr.RemoveCustomEvent(this, EventTriggerType.PointerClick);
            EventMgr.Instance.RemoveListener<int>(ClientEvent.LEVEL_PRE_UP, OnPreLvlUp);
            btn_sub.onClick.RemoveListener(OnSubClick);
        }

        public void SetData(BagItemInfo info)
        {
            UIMgr.AddCustomEventListener(this, EventTriggerType.PointerClick, OnClick);
            EventMgr.Instance.AddListener<int>(ClientEvent.LEVEL_PRE_UP, OnPreLvlUp);
            btn_sub.onClick.RemoveListener(OnSubClick);
            btn_sub.onClick.AddListener(OnSubClick);
            
            _num = 0;
            _preLv = 0;
            _bagNum = info.num;
            _itemId = info.id;
            
            normal_item.SetData(info);
            txt_num.text = 0.ToString();
            img_choose.gameObject.SetActive(false);
            btn_sub.gameObject.SetActive(false);
        }

        private void OnPreLvlUp(int toLv) => _preLv = toLv;

        private void OnClick(BaseEventData data)
        {
            ++_num;
            if (_num > _bagNum || _preLv >= StudentMgr.Instance.MaxLvl)
            {
                --_num;
                return;
            }
            
            img_choose.gameObject.SetActive(true);
            btn_sub.gameObject.SetActive(true);
            txt_num.text = _num.ToString();
            EventMgr.Instance.TriggerEvent(ClientEvent.EXP_ADD, _itemId, 1);
        }

        private void OnSubClick()
        {
            --_num;
            if (_num <= 0)
            {
                _num = 0;
                img_choose.gameObject.SetActive(false);
                btn_sub.gameObject.SetActive(false);
            }
            
            txt_num.text = _num.ToString();
            EventMgr.Instance.TriggerEvent(ClientEvent.EXP_ADD, _itemId, -1);
        }
    }
}