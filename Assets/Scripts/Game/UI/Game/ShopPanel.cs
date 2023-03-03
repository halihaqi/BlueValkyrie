using Game.UI.Controls;
using Hali_Framework;

namespace Game.UI.Game
{
    public class ShopPanel : PanelBase
    {
        private UI_hub_form _hubForm;
        private HList _svShopList;
        private HList _svShop;

        protected internal override void OnInit(object userData)
        {
            IsFullScreen = true;
            base.OnInit(userData);
            _hubForm = GetControl<UI_hub_form>("hub_form");
            _svShopList = GetControl<HList>("sv_shop_list");
            _svShop = GetControl<HList>("sv_shop");
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
            _hubForm.SetData(HideMe);
        }
    }
}