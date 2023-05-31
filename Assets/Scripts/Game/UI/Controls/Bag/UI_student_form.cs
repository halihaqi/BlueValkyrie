using Game.Model;
using Hali_Framework;

namespace Game.UI.Controls
{
    public partial class UI_student_form : ControlBase
    {
        private StudentItem _curStudent;
        private int _chooseIndex = 0;

        protected internal override void OnInit()
        {
            base.OnInit();
            tog_equip.onValueChanged.AddListener(OnEquipOpen);
            tog_lvl.onValueChanged.AddListener(OnLvlOpen);
            tog_star.onValueChanged.AddListener(OnStarOpen);
        }

        public void SetData(StudentItem student)
        {
            _curStudent = student;
            attribute_group.SetData(student);
            switch (_chooseIndex)
            {
                case 0:
                    tog_equip.isOn = true;
                    OnEquipOpen(true);
                    break;
                case 1:
                    tog_lvl.isOn = true;
                    OnLvlOpen(true);
                    break;
                case 2:
                    tog_star.isOn = true;
                    OnStarOpen(true);
                    break;
            }
        }

        private void OnEquipOpen(bool isOn)
        {
            if(!isOn) return;
            equip_group.gameObject.SetActive(true);
            lv_group.gameObject.SetActive(false);
            star_group.gameObject.SetActive(false);
            attribute_group.SetData(_curStudent);
            equip_group.SetData(_curStudent);
            _chooseIndex = 0;
        }
        
        private void OnLvlOpen(bool isOn)
        {
            if(!isOn) return;
            lv_group.gameObject.SetActive(true);
            equip_group.gameObject.SetActive(false);
            star_group.gameObject.SetActive(false);
            attribute_group.SetData(_curStudent);
            lv_group.SetData(_curStudent);
            _chooseIndex = 1;
        }
        
        private void OnStarOpen(bool isOn)
        {
            if(!isOn) return;
            star_group.gameObject.SetActive(true);
            lv_group.gameObject.SetActive(false);
            equip_group.gameObject.SetActive(false);
            star_group.SetData(_curStudent);
            _chooseIndex = 2;
        }
    }
}