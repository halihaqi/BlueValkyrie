using Hali_Framework;
using UnityEngine.UI;
namespace Game.UI.Controls
{
	public partial class UI_reward_item : ControlBase
	{
		private UI_normal_item normal_item;
		private Image img_rare;

		protected override void BindControls()
		{
			base.BindControls();
			normal_item = GetControl<UI_normal_item>("normal_item");
			img_rare = GetControl<Image>("img_rare");
		}
	}
}