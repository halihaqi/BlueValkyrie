using Hali_Framework;
using UnityEngine.UI;
namespace Game.UI.Controls
{
	public partial class UI_student_info_small : ControlBase
	{
		private Image img_head;
		private Text txt_star;
		private Text txt_lv;

		protected override void BindControls()
		{
			base.BindControls();
			img_head = GetControl<Image>("img_head");
			txt_star = GetControl<Text>("txt_star");
			txt_lv = GetControl<Text>("txt_lv");
		}
	}
}