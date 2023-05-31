using Game.Entity;
using Game.Managers;
using Game.Model;
using Game.Utils;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public partial class UI_roleInfo_form : ControlBase
    {
        private const int STAR_WIDTH = 147;

        private StudentItem _student;
        private RoleInfo _roleInfo;

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
            txt_name.text = _roleInfo.fullName;
            txt_name.text = _roleInfo.belong;
            UIMgr.Instance.BindStageRT(role_container);
            UIMgr.Instance.SetStageSize(1.08f);
            UpdateView();
            UIMgr.Instance.RecycleAllModel();
            UIMgr.Instance.ShowModel(ResPath.GetStudentObj(_roleInfo), OnRoleLoad);
        }

        private void UpdateView()
        {
            txt_lv.text = _student.lv.ToLv();
            var maxExp = StudentMgr.Instance.GetLvlUpExp(_student.lv);
            txt_exp.text = $"{_student.exp}/{maxExp}";
            sld_exp.maxValue = maxExp;
            sld_exp.value = _student.exp;
            img_star.rectTransform.sizeDelta =
                new Vector2(STAR_WIDTH * _student.star, img_star.rectTransform.sizeDelta.y);
        }

        private void OnRoleLoad(GameObject obj)
        {
            if (!obj.TryGetComponent<FormationRoleEntity>(out var entity))
                entity = obj.AddComponent<FormationRoleEntity>();
            entity.SetRoleInfo(_roleInfo);
        }
    }
}