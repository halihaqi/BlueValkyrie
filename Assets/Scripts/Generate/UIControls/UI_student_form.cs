using Hali_Framework;
using UnityEngine.UI;
namespace Game.UI.Controls
{
	public partial class UI_student_form : ControlBase
	{
		private Toggle tog_equip;
		private Toggle tog_lvl;
		private Toggle tog_star;
		private UI_attribute_group attribute_group;
		private UI_equip_group equip_group;
		private UI_lv_group lv_group;
		private UI_star_group star_group;

		protected override void BindControls()
		{
			base.BindControls();
			tog_equip = GetControl<Toggle>("tog_equip");
			tog_lvl = GetControl<Toggle>("tog_lvl");
			tog_star = GetControl<Toggle>("tog_star");
			attribute_group = GetControl<UI_attribute_group>("attribute_group");
			equip_group = GetControl<UI_equip_group>("equip_group");
			lv_group = GetControl<UI_lv_group>("lv_group");
			star_group = GetControl<UI_star_group>("star_group");
		}
	}
}