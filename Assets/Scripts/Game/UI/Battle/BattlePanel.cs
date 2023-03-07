using Game.Managers;
using Game.UI.Controls.Battle;
using Game.Utils;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Battle
{
    public class BattlePanel : PanelBase
    {
        private Text _txtName;
        private Image _imgHead;
        private Slider _sldHp;
        private Slider _sldAp;
        private Text _txtHp;
        private Text _txtAp;
        
        private Text _txtAmmo;
        private Text _txtMaxAmmo;
        private Image _imgGun;
        private Text _txtRound;
        private Text _txtRoundNum;
        
        private UI_battle_role_info _nextRole;
        private UI_battle_role_info _secondRole;

        private BattleRoleInfo _info;
        private RoleInfo _roleInfo;

        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);
            _imgHead = GetControl<Image>("img_head");
            _imgGun = GetControl<Image>("img_gun");
            _nextRole = GetControl<UI_battle_role_info>("next_role");
            _secondRole = GetControl<UI_battle_role_info>("second_role");
            _sldHp = GetControl<Slider>("sld_hp");
            _sldAp = GetControl<Slider>("sld_ap");
            _txtHp = GetControl<Text>("txt_hp");
            _txtAp = GetControl<Text>("txt_ap");
            _txtName = GetControl<Text>("txt_name");
            _txtMaxAmmo = GetControl<Text>("txt_max_ammo");
            _txtAmmo = GetControl<Text>("txt_ammo");
            _txtRound = GetControl<Text>("txt_round");
            _txtRoundNum = GetControl<Text>("txt_round_num");
            
            _sldHp.onValueChanged.AddListener(OnHpChanged);
            _sldAp.onValueChanged.AddListener(OnApChanged);
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            _sldHp.onValueChanged.RemoveListener(OnHpChanged);
            _sldAp.onValueChanged.RemoveListener(OnApChanged);
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
            if (userData is BattleRoleInfo p)
            {
                _info = p;
                _roleInfo = RoleMgr.Instance.GetRole(_info.id);
            }

            UpdateView();
            EventMgr.Instance.AddListener<float>(ClientEvent.BATTLE_ROLE_MOVE, OnMove);
            EventMgr.Instance.AddListener<BattleRoleInfo>(ClientEvent.BATTLE_ROLE_CHANGE, OnRoleChange);
        }

        protected internal override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            EventMgr.Instance.RemoveListener<float>(ClientEvent.BATTLE_ROLE_MOVE, OnMove);
            EventMgr.Instance.RemoveListener<BattleRoleInfo>(ClientEvent.BATTLE_ROLE_CHANGE, OnRoleChange);
        }

        private void UpdateView()
        {
            _txtName.text = _roleInfo.fullName;
            _txtAmmo.text = _txtMaxAmmo.text = _info.baseAmmo.ToString();
            ResMgr.Instance.LoadAsync<Sprite>(GameConst.RES_GROUP_UI, ResPath.GetStudentIcon(_roleInfo), img =>
            {
                _imgHead.sprite = img;
            });
            _sldHp.value = _sldHp.maxValue = _info.baseHp;
            _sldAp.value = _sldAp.maxValue = _info.baseAp;
        }

        private void OnRoleChange(BattleRoleInfo curRole)
        {
            _info = curRole;
            _roleInfo = RoleMgr.Instance.GetRole(_info.id);
            UpdateView();
        }

        private void OnMove(float val)
        {
            _sldAp.value = val;
        }

        private void OnHpChanged(float val) => _txtHp.text = $"{(int)val}/{_info.baseHp}";
        
        private void OnApChanged(float val) => _txtAp.text = $"{(int)val}/{_info.baseAp}";
    }
}