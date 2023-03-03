using Game.Utils;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public class UI_btn_bag_item : ControlBase
    {
        private ItemInfo _item;
        private int _itemNum;
        
        private Image _imgItem;
        private Image _imgChoose;
        private Text _txtNum;

        public ItemInfo Item => _item;
        public int ItemNum => _itemNum;

        protected internal override void OnInit()
        {
            base.OnInit();
            _imgItem = GetControl<Image>("img_item");
            _imgChoose = GetControl<Image>("img_choose");
            _txtNum = GetControl<Text>("txt_num");
            _imgChoose.gameObject.SetActive(false);
            
            EventMgr.Instance.AddListener<ItemInfo, int>(ClientEvent.BAG_ITEM_CLICK, OnItemClick);
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            EventMgr.Instance.RemoveListener<ItemInfo, int>(ClientEvent.BAG_ITEM_CLICK, OnItemClick);
        }

        public void SetData(int itemId, int num)
        {
            _item = BinaryDataMgr.Instance.GetInfo<ItemInfoContainer, int, ItemInfo>(itemId);
            _itemNum = num;
            SetNull(_item == null);
            
            if(_item == null) return;
            ResMgr.Instance.LoadAsync<Sprite>(GameConst.RES_GROUP_UI, ResPath.GetItemIcon(_item), img =>
            {
                _imgItem.sprite = img;
            });
            _txtNum.text = num.ToXNum();
        }

        private void SetNull(bool isNull)
        {
            _imgItem.gameObject.SetActive(!isNull);
            _txtNum.gameObject.SetActive(!isNull);
        }
        
        private void OnItemClick(ItemInfo info, int num)
        {
            _imgChoose.gameObject.SetActive(info.id == _item.id);
        }
    }
}