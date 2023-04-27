using Hali_Framework;
using UnityEngine.UI;
namespace Game.UI.Controls
{
	public partial class UI_btn_normal : ControlBase
	{
		private Text txt_name;

		protected override void BindControls()
		{
			base.BindControls();
			txt_name = GetControl<Text>("txt_name");
		}
	}
}