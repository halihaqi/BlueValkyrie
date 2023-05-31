using Game.Managers;
using Game.UI.Game;
using Game.Utils;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public partial class UI_shop_item : ControlBase
    {
        private ShopItemInfo _shopItem;
        private int _inventoryNum;

        protected internal override void OnInit()
        {
            base.OnInit();
            btn_buy.onClick.AddListener(OnBtnBuyClick);
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            btn_buy.onClick.RemoveListener(OnBtnBuyClick);
        }

        public void SetData(ShopItemInfo info, int num)
        {
            _shopItem = info;
            _inventoryNum = num;
            
            txt_name.text = info.name;
            normal_item.SetData(info.itemId, num);
            ResMgr.Instance.LoadAsync<Sprite>(GameConst.RES_GROUP_UI, ResPath.GetItemIcon(info.currencyId), img =>
            {
                img_cost_item.sprite = img;
            });
            txt_cost_num.text = info.price.ToXNum();
        }

        public void SetData(int shopItemId, int num)
        {
            var info = ShopMgr.Instance.GetShopItem(shopItemId);
            SetData(info, num);
        }

        private void OnBtnBuyClick()
        {
            UIMgr.Instance.ShowPanel<ShopBuyPop>(GameConst.UIGROUP_POP,
                userData: new ShopBuyParam(_shopItem, _inventoryNum));
        }
    }
}