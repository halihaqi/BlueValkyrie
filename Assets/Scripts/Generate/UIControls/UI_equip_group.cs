using Hali_Framework;
using UnityEngine.UI;
namespace Game.UI.Controls
{
	public partial class UI_equip_group : ControlBase
	{
		private UI_equip_item equip_hat;
		private UI_equip_item equip_bag;
		private UI_equip_item equip_gloves;
		private UI_equip_item equip_shoes;

		protected override void BindControls()
		{
			base.BindControls();
			equip_hat = GetControl<UI_equip_item>("equip_hat");
			equip_bag = GetControl<UI_equip_item>("equip_bag");
			equip_gloves = GetControl<UI_equip_item>("equip_gloves");
			equip_shoes = GetControl<UI_equip_item>("equip_shoes");
		}
	}
}