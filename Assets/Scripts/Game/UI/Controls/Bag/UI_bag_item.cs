using Game.Managers;
using Game.Utils;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public partial class UI_bag_item : ControlBase
    {
        private ItemInfo _item;
        private int _itemNum;

        public ItemInfo Item => _item;
        public int ItemNum => _itemNum;

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            EventMgr.Instance.RemoveListener<ItemInfo, int>(ClientEvent.BAG_ITEM_CLICK, OnItemClick);
        }

        public void SetData(int itemId, int num)
        {
            img_choose.gameObject.SetActive(false);
            EventMgr.Instance.AddListener<ItemInfo, int>(ClientEvent.BAG_ITEM_CLICK, OnItemClick);
            
            _item = ItemMgr.Instance.GetItem(itemId);
            _itemNum = num;
            SetNull(_item == null);
            
            if(_item == null) return;
            ResMgr.Instance.LoadAsync<Sprite>(GameConst.RES_GROUP_UI, ResPath.GetItemIcon(_item), img =>
            {
                img_item.sprite = img;
            });
            txt_num.text = num.ToXNum();
        }

        private void SetNull(bool isNull)
        {
            img_item.gameObject.SetActive(!isNull);
            txt_num.gameObject.SetActive(!isNull);
        }
        
        private void OnItemClick(ItemInfo info, int num)
        {
            img_choose.gameObject.SetActive(info.id == _item.id);
        }
    }
}