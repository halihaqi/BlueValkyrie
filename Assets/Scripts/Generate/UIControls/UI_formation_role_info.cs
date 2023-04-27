using Hali_Framework;
using UnityEngine.UI;
namespace Game.UI.Controls
{
	public partial class UI_formation_role_info : ControlBase
	{
		private Image img_star;
		private Text txt_star;
		private Text txt_lv;
		private Text txt_name;

		protected override void BindControls()
		{
			base.BindControls();
			img_star = GetControl<Image>("img_star");
			txt_star = GetControl<Text>("txt_star");
			txt_lv = GetControl<Text>("txt_lv");
			txt_name = GetControl<Text>("txt_name");
		}
	}
}