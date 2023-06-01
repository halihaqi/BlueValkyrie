using Hali_Framework;
using UnityEngine.UI;
namespace Game.UI.Controls
{
	public partial class UI_role_chess : ControlBase
	{
		private Image img_chess;
		private Image img_ring;

		protected override void BindControls()
		{
			base.BindControls();
			img_chess = GetControl<Image>("img_chess");
			img_ring = GetControl<Image>("img_ring");
		}
	}
}