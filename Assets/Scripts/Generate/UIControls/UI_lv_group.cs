using Hali_Framework;
using UnityEngine.UI;
namespace Game.UI.Controls
{
	public partial class UI_lv_group : ControlBase
	{
		private Slider sld_exp;
		private Slider sld_exp_add;
		private HList sv_lv;
		private Text txt_lv;
		private Text txt_exp;
		private Text txt_cost;
		private UI_btn_big btn_lv;

		protected override void BindControls()
		{
			base.BindControls();
			sld_exp = GetControl<Slider>("sld_exp");
			sld_exp_add = GetControl<Slider>("sld_exp_add");
			sv_lv = GetControl<HList>("sv_lv");
			txt_lv = GetControl<Text>("txt_lv");
			txt_exp = GetControl<Text>("txt_exp");
			txt_cost = GetControl<Text>("txt_cost");
			btn_lv = GetControl<UI_btn_big>("btn_lv");
		}
	}
}