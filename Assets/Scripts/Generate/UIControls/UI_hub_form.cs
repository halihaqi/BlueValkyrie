using Hali_Framework;
using UnityEngine.UI;
namespace Game.UI.Controls
{
	public partial class UI_hub_form : ControlBase
	{
		private Button btn_back;
		private Text txt_gold;
		private Text txt_gem;

		protected override void BindControls()
		{
			base.BindControls();
			btn_back = GetControl<Button>("btn_back");
			txt_gold = GetControl<Text>("txt_gold");
			txt_gem = GetControl<Text>("txt_gem");
		}
	}
}