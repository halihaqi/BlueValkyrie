using Game.Managers;
using Game.Model;
using Hali_Framework;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public partial class UI_formation_role_info : ControlBase
    {
        public void SetData(StudentItem student)
        {
            if (student == null)
            {
                txt_name.text = "NULL";
                txt_star.gameObject.SetActive(false);
                img_star.gameObject.SetActive(false);
                txt_lv.gameObject.SetActive(false);
            }
            else
            {
                txt_star.gameObject.SetActive(true);
                img_star.gameObject.SetActive(true);
                txt_lv.gameObject.SetActive(true);
                txt_star.text = student.star.ToString();
                txt_lv.text = student.lv.ToLv();
                txt_name.text = RoleMgr.Instance.GetRole(student.roleId).fullName;
            }
        }
    }
}