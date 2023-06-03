using Hali_Framework;
using UnityEngine.UI;
namespace Game.UI.Controls
{
	public partial class UI_self_state : ControlBase
	{
		private Image img_weapon;
		private Image img_formation;
		private Slider sld_hp;
		private Text txt_weapon_type;
		private Text txt_ammo;
		private Text txt_hp;
		private Text txt_name;

		protected override void BindControls()
		{
			base.BindControls();
			img_weapon = GetControl<Image>("img_weapon");
			img_formation = GetControl<Image>("img_formation");
			sld_hp = GetControl<Slider>("sld_hp");
			txt_weapon_type = GetControl<Text>("txt_weapon_type");
			txt_ammo = GetControl<Text>("txt_ammo");
			txt_hp = GetControl<Text>("txt_hp");
			txt_name = GetControl<Text>("txt_name");
		}
	}
}