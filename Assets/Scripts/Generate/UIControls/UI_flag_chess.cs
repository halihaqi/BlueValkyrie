using Hali_Framework;
using UnityEngine.UI;
namespace Game.UI.Controls
{
	public partial class UI_flag_chess : ControlBase
	{
		private Image none_chess;
		private Image student_chess;
		private Image enemy_chess;

		protected override void BindControls()
		{
			base.BindControls();
			none_chess = GetControl<Image>("none_chess");
			student_chess = GetControl<Image>("student_chess");
			enemy_chess = GetControl<Image>("enemy_chess");
		}
	}
}