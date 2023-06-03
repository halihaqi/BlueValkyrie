using Hali_Framework;
using UnityEngine.UI;
namespace Game.UI.Controls
{
	public partial class UI_tip_btn : ControlBase
	{
		private Text txt_tip;
		private Text txt_key;

		protected override void BindControls()
		{
			base.BindControls();
			txt_tip = GetControl<Text>("txt_tip");
			txt_key = GetControl<Text>("txt_key");
		}
	}
}