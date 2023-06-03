using Game.BattleScene.BattleRole;
using Game.UI.Controls;
using Hali_Framework;
using UnityEngine;

namespace Game.UI.Battle
{
    public class BattlePanel : PanelBase
    {
        private IBattleRole _role;
        private UI_self_state _selfState;
        private UI_move_group _moveGroup;
        private UI_shoot_group _shootGroup;

        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);
            _selfState = GetControl<UI_self_state>("self_state");
            _moveGroup = GetControl<UI_move_group>("move_group");
            _shootGroup = GetControl<UI_shoot_group>("shoot_group");
        }

        protected internal override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
            if (userData is IBattleRole role)
                _role = role;
            else
                Debug.Log("BattlePanel Has no role info.");
            
            Visible = true;
            PanelEntity.Fade(true);
            _selfState.SetData(_role);
            _moveGroup.SetData(_role, OnAim, OnMap);
            _shootGroup.SetData(OnFire);
            ChangeMode(true);
        }
        
        protected internal override void OnCover()
        {
            base.OnCover();
            Visible = false;
        }

        private void OnAim()
        {
            
        }

        private void OnMap()
        {
            
        }
        
        private void OnFire()
        {
            
        }

        public void ChangeMode(bool moveOrShoot)
        {
            _moveGroup.SetAlpha(moveOrShoot);
            _shootGroup.SetAlpha(!moveOrShoot);
        }
    }
}