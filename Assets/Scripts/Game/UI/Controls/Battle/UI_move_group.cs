using Game.Entity;
using Hali_Framework;
using UnityEngine.UI;

namespace Game.UI.Controls.Battle
{
    public class UI_move_group : ControlBase
    {
        private BattleRoleEntity _curRole;
        private Slider _sldAp;
        private Image _tipFlag;
        private Image _tipAim;
        private Image _tipOver;

        protected internal override void OnInit()
        {
            base.OnInit();
            _sldAp = GetControl<Slider>("sld_ap");
            _tipFlag = GetControl<Image>("tip_flag");
            _tipAim = GetControl<Image>("tip_aim");
            _tipOver = GetControl<Image>("tip_over");
        }

        public void SetData(BattleRoleEntity role)
        {
            _curRole = role;
            _sldAp.maxValue = role.MaxAp;
            _sldAp.value = role.CurAp;
            _tipFlag.gameObject.SetActive(!role.IsEnemy);
            _tipAim.gameObject.SetActive(!role.IsEnemy);
            _tipOver.gameObject.SetActive(!role.IsEnemy);
        }

        public void OnMove()
        {
            _sldAp.value = _curRole.CurAp;
        }

        public void SetFlagTip(bool isActive)
        {
            _tipFlag.gameObject.SetActive(isActive);
        }

        public void SetActive(bool isActive)
        {
            this.gameObject.SetActive(isActive);
        }
    }
}