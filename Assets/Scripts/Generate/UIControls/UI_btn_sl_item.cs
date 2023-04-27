using Hali_Framework;
using UnityEngine.UI;
namespace Game.UI.Controls
{
	public partial class UI_btn_sl_item : ControlBase
	{
		private Image img_badge;
		private Image img_title_save;
		private Image img_title_load;
		private Slider sld_complete;
		private Text txt_title;
		private Text txt_name;
		private Text txt_time;

		protected override void BindControls()
		{
			base.BindControls();
			img_badge = GetControl<Image>("img_badge");
			img_title_save = GetControl<Image>("img_title_save");
			img_title_load = GetControl<Image>("img_title_load");
			sld_complete = GetControl<Slider>("sld_complete");
			txt_title = GetControl<Text>("txt_title");
			txt_name = GetControl<Text>("txt_name");
			txt_time = GetControl<Text>("txt_time");
		}
	}
}