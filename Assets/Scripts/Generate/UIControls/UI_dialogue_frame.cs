using Hali_Framework;
using UnityEngine.UI;
namespace Game.UI.Controls
{
	public partial class UI_dialogue_frame : ControlBase
	{
		private Image img_next;
		private Text txt_dialogue;
		private Text txt_hide;

		protected override void BindControls()
		{
			base.BindControls();
			img_next = GetControl<Image>("img_next");
			txt_dialogue = GetControl<Text>("txt_dialogue");
			txt_hide = GetControl<Text>("txt_hide");
		}
	}
}