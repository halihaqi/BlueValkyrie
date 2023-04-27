using Hali_Framework;
using UnityEngine.UI;
namespace Game.UI.Controls
{
	public partial class UI_btn_big : ControlBase
	{
		private Image img_alpha;
		private Text txt_name;

		protected override void BindControls()
		{
			base.BindControls();
			img_alpha = GetControl<Image>("img_alpha");
			txt_name = GetControl<Text>("txt_name");
		}
	}
}