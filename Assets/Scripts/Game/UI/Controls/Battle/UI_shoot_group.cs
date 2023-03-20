using Game.Entity;
using Game.Model;
using Game.Utils;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Controls.Battle
{
    public class UI_shoot_group : ControlBase
    {
        private Image _imgGun;
        private Image _imgNormalPoint;
        private Image _imgFocusPoint;
        private Text _txtMaxAmmo;
        private Text _txtAmmo;
        private Image _tipFire;
        private Image _tipOver;

        protected internal override void OnInit()
        {
            base.OnInit();
            _imgGun = GetControl<Image>("img_gun");
            _imgNormalPoint = GetControl<Image>("img_normal_point");
            _imgFocusPoint = GetControl<Image>("img_focus_point");
            _txtMaxAmmo = GetControl<Text>("txt_max_ammo");
            _txtAmmo = GetControl<Text>("txt_ammo");
            _tipFire = GetControl<Image>("tip_fire");
            _tipOver = GetControl<Image>("tip_over");
        }

        public void SetData(BattleRoleEntity role)
        {
            ResMgr.Instance.LoadAsync<Sprite>(GameConst.RES_GROUP_UI, ResPath.GetGunIcon(role.AtkType), img =>
            {
                _imgGun.sprite = img;
            });
            SwitchArrow(false);
            _txtMaxAmmo.text = $"/{role.MaxAmmo}";
            _txtAmmo.text = role.CurAmmo <= 0 ? $"<color=red>{role.CurAmmo}</color>" : role.CurAmmo.ToString();
            _tipFire.gameObject.SetActive(!role.IsEnemy);
            _tipOver.gameObject.SetActive(!role.IsEnemy);
        }
        
        public void SetActive(bool isActive)
        {
            this.gameObject.SetActive(isActive);
        }

        public void SwitchArrow(bool isFocus)
        {
            _imgNormalPoint.gameObject.SetActive(!isFocus);
            _imgFocusPoint.gameObject.SetActive(isFocus);
        }
    }
}