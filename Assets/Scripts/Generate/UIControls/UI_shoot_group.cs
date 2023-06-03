using Hali_Framework;
using UnityEngine.UI;
namespace Game.UI.Controls
{
	public partial class UI_shoot_group : ControlBase
	{
		private Image img_normal_point;
		private Image img_focus_point;
		private UI_tip_btn tip_fire;

		protected override void BindControls()
		{
			base.BindControls();
			img_normal_point = GetControl<Image>("img_normal_point");
			img_focus_point = GetControl<Image>("img_focus_point");
			tip_fire = GetControl<UI_tip_btn>("tip_fire");
		}
	}
}