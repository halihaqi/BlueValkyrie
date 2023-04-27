using Hali_Framework;
using UnityEngine.UI;
namespace Game.UI.Controls
{
	public partial class UI_equip_item : ControlBase
	{
		private Text txt_equip_type;
		private Image img_full;
		private Text txt_attri;
		private Text txt_attri_num;
		private UI_normal_item normal_item;
		private Image img_empty;

		protected override void BindControls()
		{
			base.BindControls();
			txt_equip_type = GetControl<Text>("txt_equip_type");
			img_full = GetControl<Image>("img_full");
			txt_attri = GetControl<Text>("txt_attri");
			txt_attri_num = GetControl<Text>("txt_attri_num");
			normal_item = GetControl<UI_normal_item>("normal_item");
			img_empty = GetControl<Image>("img_empty");
		}
	}
}