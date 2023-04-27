using Hali_Framework;
using UnityEngine.UI;
namespace Game.UI.Controls
{
	public partial class UI_star_item : ControlBase
	{
		private UI_normal_item normal_item;
		private Text txt_num;
		private Image img_empty;

		protected override void BindControls()
		{
			base.BindControls();
			normal_item = GetControl<UI_normal_item>("normal_item");
			txt_num = GetControl<Text>("txt_num");
			img_empty = GetControl<Image>("img_empty");
		}
	}
}