using Game.Managers;
using Game.Model.BagModel;
using Hali_Framework;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI.Controls.LevelUp
{
    public partial class UI_lv_item : ControlBase
    {
        private UI_normal_item _normalItem;
        private Text _txtNum;
        private Image _imgChoose;
        private Button _btnSub;

        private int _num = 0;
        private int _bagNum;
        private int _itemId;

        private int _preLv;
        
        protected internal override void OnInit()
        {
            base.OnInit();
            _normalItem = GetControl<UI_normal_item>("normal_item");
            _txtNum = GetControl<Text>("txt_num");
            _imgChoose = GetControl<Image>("img_choose");
            _btnSub = GetControl<Button>("btn_sub");
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            UIMgr.RemoveCustomEvent(this, EventTriggerType.PointerClick);
            EventMgr.Instance.RemoveListener<int>(ClientEvent.LEVEL_PRE_UP, OnPreLvlUp);
            _btnSub.onClick.RemoveListener(OnSubClick);
        }

        public void SetData(BagItemInfo info)
        {
            UIMgr.AddCustomEventListener(this, EventTriggerType.PointerClick, OnClick);
            EventMgr.Instance.AddListener<int>(ClientEvent.LEVEL_PRE_UP, OnPreLvlUp);
            _btnSub.onClick.RemoveListener(OnSubClick);
            _btnSub.onClick.AddListener(OnSubClick);
            
            _num = 0;
            _preLv = 0;
            _bagNum = info.num;
            _itemId = info.id;
            
            _normalItem.SetData(info);
            _txtNum.text = 0.ToString();
            _imgChoose.gameObject.SetActive(false);
            _btnSub.gameObject.SetActive(false);
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
            
            _imgChoose.gameObject.SetActive(true);
            _btnSub.gameObject.SetActive(true);
            _txtNum.text = _num.ToString();
            EventMgr.Instance.TriggerEvent(ClientEvent.EXP_ADD, _itemId, 1);
        }

        private void OnSubClick()
        {
            --_num;
            if (_num <= 0)
            {
                _num = 0;
                _imgChoose.gameObject.SetActive(false);
                _btnSub.gameObject.SetActive(false);
            }
            
            _txtNum.text = _num.ToString();
            EventMgr.Instance.TriggerEvent(ClientEvent.EXP_ADD, _itemId, -1);
        }
    }
}