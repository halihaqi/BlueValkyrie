using Game.Managers;
using Game.UI.Game;
using Game.Utils;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public class UI_shop_item : ControlBase
    {
        private Button _btnBuy;
        private Text _txtName;
        private Text _txtCostNum;
        private Image _imgCostItem;
        private UI_normal_item _item;

        private ShopItemInfo _shopItem;
        private int _inventoryNum;

        protected internal override void OnInit()
        {
            base.OnInit();
            _btnBuy = GetControl<Button>("btn_buy");
            _txtName = GetControl<Text>("txt_name");
            _txtCostNum = GetControl<Text>("txt_cost_num");
            _imgCostItem = GetControl<Image>("img_cost_item");
            _item = GetControl<UI_normal_item>("normal_item");
            _btnBuy.onClick.AddListener(OnBtnBuyClick);
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            _btnBuy.onClick.RemoveListener(OnBtnBuyClick);
        }

        public void SetData(ShopItemInfo info, int num)
        {
            _shopItem = info;
            _inventoryNum = num;
            
            _txtName.text = info.name;
            _item.SetData(info.itemId, num);
            ResMgr.Instance.LoadAsync<Sprite>(GameConst.RES_GROUP_UI, ResPath.GetItemIcon(info.currencyId), img =>
            {
                _imgCostItem.sprite = img;
            });
            _txtCostNum.text = info.price.ToXNum();
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