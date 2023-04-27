using Hali_Framework;
using UnityEngine.UI;
namespace Game.UI.Controls
{
	public partial class UI_normal_item : ControlBase
	{
		private Image img_item;
		private Text txt_num;

		protected override void BindControls()
		{
			base.BindControls();
			img_item = GetControl<Image>("img_item");
			txt_num = GetControl<Text>("txt_num");
		}
	}
}