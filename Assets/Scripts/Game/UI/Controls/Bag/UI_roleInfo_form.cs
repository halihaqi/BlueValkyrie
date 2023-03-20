using Game.Entity;
using Game.Managers;
using Game.Model;
using Game.Utils;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public class UI_roleInfo_form : ControlBase
    {
        private const int STAR_WIDTH = 147;

        private RawImage _roleContainer;
        private Image _imgStar;
        private Slider _sldExp;
        private Text _txtSchool;
        private Text _txtName;
        private Text _txtLv;
        private Text _txtExp;

        private StudentItem _student;
        private RoleInfo _roleInfo;

        protected internal override void OnInit()
        {
            base.OnInit();
            _txtName = GetControl<Text>("txt_name");
            _txtSchool = GetControl<Text>("txt_school");
            _txtLv = GetControl<Text>("txt_lv");
            _txtExp = GetControl<Text>("txt_exp");
            _sldExp = GetControl<Slider>("sld_exp");
            _imgStar = GetControl<Image>("img_star");
            _roleContainer = GetControl<RawImage>("role_container");
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            UIMgr.Instance.ClearModel();
            EventMgr.Instance.RemoveListener(ClientEvent.EXP_UP, UpdateView);
            EventMgr.Instance.RemoveListener(ClientEvent.STAR_UP, UpdateView);
        }

        public void SetData(StudentItem student)
        {
            EventMgr.Instance.AddListener(ClientEvent.EXP_UP, UpdateView);
            EventMgr.Instance.AddListener(ClientEvent.STAR_UP, UpdateView);

            _student = student;
            _roleInfo = RoleMgr.Instance.GetRole(student.roleId);
            _txtName.text = _roleInfo.fullName;
            _txtSchool.text = _roleInfo.belong;
            UIMgr.Instance.BindStageRT(_roleContainer);
            UIMgr.Instance.SetStageSize(1.08f);
            UpdateView();
            UIMgr.Instance.RecycleAllModel();
            UIMgr.Instance.ShowModel(ResPath.GetStudentObj(_roleInfo), OnRoleLoad);
        }

        private void UpdateView()
        {
            _txtLv.text = _student.lv.ToLv();
            var maxExp = StudentMgr.Instance.GetLvlUpExp(_student.lv);
            _txtExp.text = $"{_student.exp}/{maxExp}";
            _sldExp.maxValue = maxExp;
            _sldExp.value = _student.exp;
            _imgStar.rectTransform.sizeDelta = new Vector2(STAR_WIDTH * _student.star, _imgStar.rectTransform.sizeDelta.y);
        }

        private void OnRoleLoad(GameObject obj)
        {
            if (!obj.TryGetComponent<FormationRoleEntity>(out var entity))
                entity = obj.AddComponent<FormationRoleEntity>();
            entity.SetRoleInfo(_roleInfo);
        }
    }
}