using Game.BattleScene.BattleRole;
using Game.Utils;
using Hali_Framework;
using UnityEngine;

namespace Game.UI.Controls
{
    public partial class UI_self_state : ControlBase
    {
        public void SetData(IBattleRole role)
        {
            ResMgr.Instance.LoadAsync<Sprite>(GameConst.RES_GROUP_UI, ResPath.GetGunIcon(role.AtkType), img =>
            {
                img_weapon.sprite = img;
            });
            ResMgr.Instance.LoadAsync<Sprite>(GameConst.RES_GROUP_UI, ResPath.GetSchoolBadgeIcon(role.RoleId), img =>
            {
                img_formation.sprite = img;
            });
            sld_hp.maxValue = role.RoleState.maxHp;
            sld_hp.value = role.CurHp;
            txt_weapon_type.text = role.AtkType.ToString();
            txt_ammo.text = $"{role.CurAmmo}/{role.RoleState.maxAmmo}";
            txt_hp.text = $"{role.CurHp}/{role.RoleState.maxHp}";
            txt_name.text = role.RoleName;
        }
    }
}