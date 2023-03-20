using Game.Managers;
using Game.Model;
using Hali_Framework;
using UnityEngine.UI;

namespace Game.UI.Controls.Formation
{
    public class UI_formation_role_info : ControlBase
    {
        private Text _txtStar;
        private Text _txtLv;
        private Text _txtName;
        private Image _imgStar;

        protected internal override void OnInit()
        {
            base.OnInit();
            _txtLv = GetControl<Text>("txt_lv");
            _txtName = GetControl<Text>("txt_name");
            _txtStar = GetControl<Text>("txt_star");
            _imgStar = GetControl<Image>("img_star");
        }

        public void SetData(StudentItem student)
        {
            if (student == null)
            {
                _txtName.text = "NULL";
                _txtStar.gameObject.SetActive(false);
                _imgStar.gameObject.SetActive(false);
                _txtLv.gameObject.SetActive(false);
            }
            else
            {
                _txtStar.gameObject.SetActive(true);
                _imgStar.gameObject.SetActive(true);
                _txtLv.gameObject.SetActive(true);
                _txtStar.text = student.star.ToString();
                _txtLv.text = student.lv.ToLv();
                _txtName.text = RoleMgr.Instance.GetRole(student.roleId).fullName;   
            }
        }
    }
}