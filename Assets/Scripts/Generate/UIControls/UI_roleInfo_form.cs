using Hali_Framework;
using UnityEngine.UI;
namespace Game.UI.Controls
{
	public partial class UI_roleInfo_form : ControlBase
	{
		private RawImage role_container;
		private Image img_star;
		private Slider sld_exp;
		private Text txt_school;
		private Text txt_name;
		private Text txt_lv;
		private Text txt_exp;

		protected override void BindControls()
		{
			base.BindControls();
			role_container = GetControl<RawImage>("role_container");
			img_star = GetControl<Image>("img_star");
			sld_exp = GetControl<Slider>("sld_exp");
			txt_school = GetControl<Text>("txt_school");
			txt_name = GetControl<Text>("txt_name");
			txt_lv = GetControl<Text>("txt_lv");
			txt_exp = GetControl<Text>("txt_exp");
		}
	}
}