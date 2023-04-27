using Hali_Framework;
using UnityEngine.UI;
namespace Game.UI.Controls
{
	public partial class UI_bag_form : ControlBase
	{
		private Button btn_sift;
		private Button btn_sort;
		private Image img_sort_arrow;
		private HList sv_bag;
		private Text txt_bag_title;

		protected override void BindControls()
		{
			base.BindControls();
			btn_sift = GetControl<Button>("btn_sift");
			btn_sort = GetControl<Button>("btn_sort");
			img_sort_arrow = GetControl<Image>("img_sort_arrow");
			sv_bag = GetControl<HList>("sv_bag");
			txt_bag_title = GetControl<Text>("txt_bag_title");
		}
	}
}