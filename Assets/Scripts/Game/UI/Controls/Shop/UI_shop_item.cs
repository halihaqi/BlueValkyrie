using Hali_Framework;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public class UI_shop_item : ControlBase
    {
        private Button _btnBuy;
        private Text _txtName;
        private Text _txtItemNum;
        private Text _txtCostNum;

        private Image _imgItem;
        private Image _imgCostItem;

        protected internal override void OnInit()
        {
            base.OnInit();
            _btnBuy = GetControl<Button>("btn_buy");
            
            _txtName = GetControl<Text>("txt_name");
            _txtItemNum = GetControl<Text>("txt_item_num");
            _txtCostNum = GetControl<Text>("txt_cost_num");
            
            _imgItem = GetControl<Image>("img_item");
            _imgCostItem = GetControl<Image>("img_cost_item");
        }
    }
}