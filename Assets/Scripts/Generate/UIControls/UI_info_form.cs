using Hali_Framework;
using UnityEngine.UI;
namespace Game.UI.Controls
{
	public partial class UI_info_form : ControlBase
	{
		private Image img_item;
		private Image attribute_group;
		private Text txt_attribute;
		private Text txt_attribute_num;
		private Text txt_info_tip;
		private Text txt_info;
		private Text txt_num;

		protected override void BindControls()
		{
			base.BindControls();
			img_item = GetControl<Image>("img_item");
			attribute_group = GetControl<Image>("attribute_group");
			txt_attribute = GetControl<Text>("txt_attribute");
			txt_attribute_num = GetControl<Text>("txt_attribute_num");
			txt_info_tip = GetControl<Text>("txt_info_tip");
			txt_info = GetControl<Text>("txt_info");
			txt_num = GetControl<Text>("txt_num");
		}
	}
}