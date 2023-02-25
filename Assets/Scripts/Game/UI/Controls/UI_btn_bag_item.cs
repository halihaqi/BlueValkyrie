using Game.Utils;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public class UI_btn_bag_item : ControlBase
    {
        private ItemInfo _item;
        
        private Button _btn;
        private Image _imgItem;
        private Image _imgChoose;
        private Text _txtNum;

        protected internal override void OnInit()
        {
            base.OnInit();
            _btn = GetControl<Button>("btn_bag_item");
            _imgItem = GetControl<Image>("img_item");
            _imgChoose = GetControl<Image>("img_choose");
            _txtNum = GetControl<Text>("txt_choose");
            _imgChoose.gameObject.SetActive(false);
            _btn.onClick.RemoveAllListeners();
            _btn.onClick.AddListener(OnClick);
        }

        public void SetData(int itemId, int num)
        {
            _item = BinaryDataMgr.Instance.GetInfo<ItemInfoContainer, int, ItemInfo>(itemId);
            SetNull(_item == null);
            
            if(_item == null) return;
            ResMgr.Instance.LoadAsync<Sprite>(GameConst.RES_GROUP_UI, ResPath.GetItemIcon(_item), img =>
            {
                _imgItem.sprite = img;
            });
            _txtNum.text = $"x{num}";
        }

        private void SetNull(bool isNull)
        {
            _imgItem.gameObject.SetActive(!isNull);
            _txtNum.gameObject.SetActive(!isNull);
        }

        private void OnClick()
        {
            //先让其他选择取消
            EventMgr.Instance.TriggerEvent(ClientEvent.BAG_ITEM_CLICK, _item);
            //选择自己
            _imgChoose.gameObject.SetActive(true);
            //添加监听其他选择的监听
            EventMgr.Instance.OnceListener<ItemInfo>(ClientEvent.BAG_ITEM_CLICK, OnOtherItemClick);
        }

        private void OnOtherItemClick(ItemInfo info)
        {
            _imgChoose.gameObject.SetActive(false);
        }
    }
}