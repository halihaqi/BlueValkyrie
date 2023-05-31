using Hali_Framework;
using UnityEngine.UI;
namespace Game.UI.Controls
{
	public partial class UI_attribute_group : ControlBase
	{
		private Text txt_hp;
		private Text txt_ap;
		private Text txt_atk;
		private Text txt_def;

		protected override void BindControls()
		{
			base.BindControls();
			txt_hp = GetControl<Text>("txt_hp");
			txt_ap = GetControl<Text>("txt_ap");
			txt_atk = GetControl<Text>("txt_atk");
			txt_def = GetControl<Text>("txt_def");
		}
	}
}