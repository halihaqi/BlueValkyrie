using Hali_Framework;
using UnityEngine.UI;
namespace Game.UI.Controls
{
	public partial class UI_battle_role_form : ControlBase
	{
		private Button btn_fight;
		private Button btn_rest;
		private Image img_top;
		private Image img_head;
		private Image img_atkType;
		private Image img_ammo;
		private Slider sld_hp;
		private Slider sld_ap;
		private Text txt_type;
		private Text txt_name;
		private Text txt_hp;
		private Text txt_ammo;

		protected override void BindControls()
		{
			base.BindControls();
			btn_fight = GetControl<Button>("btn_fight");
			btn_rest = GetControl<Button>("btn_rest");
			img_top = GetControl<Image>("img_top");
			img_head = GetControl<Image>("img_head");
			img_atkType = GetControl<Image>("img_atkType");
			img_ammo = GetControl<Image>("img_ammo");
			sld_hp = GetControl<Slider>("sld_hp");
			sld_ap = GetControl<Slider>("sld_ap");
			txt_type = GetControl<Text>("txt_type");
			txt_name = GetControl<Text>("txt_name");
			txt_hp = GetControl<Text>("txt_hp");
			txt_ammo = GetControl<Text>("txt_ammo");
		}
	}
}