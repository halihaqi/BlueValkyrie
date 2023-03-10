using Game.Model;
using Game.UI.Controls.LevelUp;
using Game.UI.Controls.StarUp;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public class UI_student_form : ControlBase
    {
        private Toggle _togEquip;
        private Toggle _togLvl;
        private Toggle _togStar;

        private UI_attribute_group _attributeGroup;
        private UI_equip_group _equipGroup;
        private UI_lv_group _lvGroup;
        private UI_star_group _starGroup;

        private StudentItem _curStudent;
        private int _chooseIndex = 0;

        protected internal override void OnInit()
        {
            base.OnInit();
            _togEquip = GetControl<Toggle>("tog_equip");
            _togLvl = GetControl<Toggle>("tog_lvl");
            _togStar = GetControl<Toggle>("tog_star");

            _attributeGroup = GetControl<UI_attribute_group>("attribute_group");
            _equipGroup = GetControl<UI_equip_group>("equip_group");
            _lvGroup = GetControl<UI_lv_group>("lv_group");
            _starGroup = GetControl<UI_star_group>("star_group");
            
            _togEquip.onValueChanged.AddListener(OnEquipOpen);
            _togLvl.onValueChanged.AddListener(OnLvlOpen);
            _togStar.onValueChanged.AddListener(OnStarOpen);
        }

        public void SetData(StudentItem student)
        {
            _curStudent = student;
            _attributeGroup.SetData(student);
            switch (_chooseIndex)
            {
                case 0:
                    _togEquip.isOn = true;
                    OnEquipOpen(true);
                    break;
                case 1:
                    _togLvl.isOn = true;
                    OnLvlOpen(true);
                    break;
                case 2:
                    _togStar.isOn = true;
                    OnStarOpen(true);
                    break;
            }
        }

        private void OnEquipOpen(bool isOn)
        {
            if(!isOn) return;
            _equipGroup.gameObject.SetActive(true);
            _lvGroup.gameObject.SetActive(false);
            _starGroup.gameObject.SetActive(false);
            _attributeGroup.SetData(_curStudent);
            _equipGroup.SetData(_curStudent);
            _chooseIndex = 0;
        }
        
        private void OnLvlOpen(bool isOn)
        {
            if(!isOn) return;
            _lvGroup.gameObject.SetActive(true);
            _equipGroup.gameObject.SetActive(false);
            _starGroup.gameObject.SetActive(false);
            _attributeGroup.SetData(_curStudent);
            _lvGroup.SetData(_curStudent);
            _chooseIndex = 1;
        }
        
        private void OnStarOpen(bool isOn)
        {
            if(!isOn) return;
            _starGroup.gameObject.SetActive(true);
            _lvGroup.gameObject.SetActive(false);
            _equipGroup.gameObject.SetActive(false);
            _starGroup.SetData(_curStudent);
            _chooseIndex = 2;
        }
    }
}