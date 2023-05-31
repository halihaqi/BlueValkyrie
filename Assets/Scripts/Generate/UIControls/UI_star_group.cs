using Hali_Framework;
using UnityEngine.UI;
namespace Game.UI.Controls
{
	public partial class UI_star_group : ControlBase
	{
		private Image img_star_old;
		private Image img_star_new;
		private UI_btn_big btn_star;
		private UI_star_item star_item_1;
		private UI_star_item star_item_2;
		private UI_star_item star_item_3;
		private UI_star_item star_item_4;
		private Text txt_exp;
		private Text txt_cost;

		protected override void BindControls()
		{
			base.BindControls();
			img_star_old = GetControl<Image>("img_star_old");
			img_star_new = GetControl<Image>("img_star_new");
			btn_star = GetControl<UI_btn_big>("btn_star");
			star_item_1 = GetControl<UI_star_item>("star_item_1");
			star_item_2 = GetControl<UI_star_item>("star_item_2");
			star_item_3 = GetControl<UI_star_item>("star_item_3");
			star_item_4 = GetControl<UI_star_item>("star_item_4");
			txt_exp = GetControl<Text>("txt_exp");
			txt_cost = GetControl<Text>("txt_cost");
		}
	}
}