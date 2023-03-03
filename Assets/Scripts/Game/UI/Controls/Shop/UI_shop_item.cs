using Hali_Framework;
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

        protected internal override void OnInit()
        {
            base.OnInit();
            _btnBuy = GetControl<Button>("btn_buy");
            _txtName = GetControl<Text>("txt_name");
            _txtCostNum = GetControl<Text>("txt_cost_num");
            _imgCostItem = GetControl<Image>("img_cost_item");
            _item = GetControl<UI_normal_item>("normal_item");
        }
    }
}