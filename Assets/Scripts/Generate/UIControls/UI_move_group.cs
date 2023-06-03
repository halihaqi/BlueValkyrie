using Hali_Framework;
using UnityEngine.UI;
namespace Game.UI.Controls
{
	public partial class UI_move_group : ControlBase
	{
		private Slider sld_ap;
		private UI_tip_btn tip_aim;
		private UI_tip_btn tip_map;

		protected override void BindControls()
		{
			base.BindControls();
			sld_ap = GetControl<Slider>("sld_ap");
			tip_aim = GetControl<UI_tip_btn>("tip_aim");
			tip_map = GetControl<UI_tip_btn>("tip_map");
		}
	}
}