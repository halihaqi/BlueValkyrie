using Game.BattleScene;
using Game.Entity;
using Game.Managers;
using Game.Utils;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Controls.Battle
{
    public partial class UI_battle_student_form : ControlBase
    {
        private Image _imgHead;
        private Image _imgAtkType;
        private Slider _sldHp;
        private Slider _sldAp;
        private Text _txtName;
        private Text _txtHp;
        private Text _txtAmmo;
        private Button _btnFight;
        private Button _btnRest;

        private BattleStudentEntity _student;

        protected internal override void OnInit()
        {
            base.OnInit();
            _imgHead = GetControl<Image>("img_head");
            _imgAtkType = GetControl<Image>("img_atkType");
            _sldHp = GetControl<Slider>("sld_hp");
            _sldAp = GetControl<Slider>("sld_ap");
            _txtName = GetControl<Text>("txt_name");
            _txtHp = GetControl<Text>("txt_hp");
            _txtAmmo = GetControl<Text>("txt_ammo");
            _btnFight = GetControl<Button>("btn_fight");
            _btnRest = GetControl<Button>("btn_rest");
            _btnFight.onClick.AddListener(OnFight);
            _btnRest.onClick.AddListener(OnRest);
        }

        public void SetData(int roleIndex)
        {
            var bm = FsmMgr.Instance.GetFsm<BattleMaster>(BattleConst.BATTLE_FSM).Owner;
            _student = bm.GetStudent(roleIndex);

            ResMgr.Instance.LoadAsync<Sprite>(GameConst.RES_GROUP_UI, ResPath.GetStudentIcon(_student.Student.roleId),
                spr =>
                {
                    _imgHead.sprite = spr;
                });
            ResMgr.Instance.LoadAsync<Sprite>(GameConst.RES_GROUP_UI, ResPath.GetGunIcon(_student.Student.AtkType),
                spr =>
                {
                    _imgAtkType.sprite = spr;
                });

            _txtName.text = RoleMgr.Instance.GetRole(_student.Student.roleId).fullName.Split(' ')[1];
            _sldHp.maxValue = _student.MaxHp;
            _sldHp.value = _student.CurHp;
            _sldAp.maxValue = _student.MaxAp;
            _sldAp.value = _student.CurAp;
            _txtHp.text = $"{_student.CurHp}/{_student.MaxHp}";
            _txtAmmo.text = $"{_student.CurAmmo}/{_student.MaxAmmo}";
        }

        private void OnFight()
        {
            TipHelper.ShowConfirm("确定要选择此角色吗？", () =>
            {
                EventMgr.Instance.TriggerEvent(ClientEvent.BATTLE_ROLE_ACTION, _student);
            });
        }

        private void OnRest()
        {
            TipHelper.ShowConfirm("确定要休整吗？", () =>
            {
                EventMgr.Instance.TriggerEvent(ClientEvent.BATTLE_ROLE_REST, _student);
            });
        }
    }
}