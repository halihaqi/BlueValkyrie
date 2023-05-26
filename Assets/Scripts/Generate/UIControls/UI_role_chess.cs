using Hali_Framework;
using UnityEngine.UI;
namespace Game.UI.Controls
{
	public partial class UI_role_chess : ControlBase
	{
		private Toggle tog_chess;
		private Image img_chess;

		protected override void BindControls()
		{
			base.BindControls();
			tog_chess = GetControl<Toggle>("tog_chess");
			img_chess = GetControl<Image>("img_chess");
		}
	}
}