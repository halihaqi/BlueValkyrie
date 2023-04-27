using Hali_Framework;
using UnityEngine.UI;
namespace Game.UI.Controls
{
	public partial class UI_shop_item : ControlBase
	{
		private UI_normal_item normal_item;
		private Button btn_buy;
		private Image img_cost_item;
		private Text txt_cost_num;
		private Text txt_name;

		protected override void BindControls()
		{
			base.BindControls();
			normal_item = GetControl<UI_normal_item>("normal_item");
			btn_buy = GetControl<Button>("btn_buy");
			img_cost_item = GetControl<Image>("img_cost_item");
			txt_cost_num = GetControl<Text>("txt_cost_num");
			txt_name = GetControl<Text>("txt_name");
		}
	}
}