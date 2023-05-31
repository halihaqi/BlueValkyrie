using Hali_Framework;
using UnityEngine.UI;
namespace Game.UI.Controls
{
	public partial class UI_round_tip_form : ControlBase
	{
		private Text txt_tip;

		protected override void BindControls()
		{
			base.BindControls();
			txt_tip = GetControl<Text>("txt_tip");
		}
	}
}