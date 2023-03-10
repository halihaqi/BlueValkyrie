using System.Collections.Generic;
using System.Linq;
using Game.Managers;
using Game.Model;
using Game.UI.Controls;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Game
{
    public class BagPanel : PanelBase
    {
        [SerializeField] private GameObject bagPanel;
        [SerializeField] private GameObject weaponPanel;

        private UI_bag_form _bagForm;
        private UI_info_form _infoForm;
        private UI_hub_form _hubForm;
        private UI_roleInfo_form _roleInfoForm;
        private UI_student_form _studentForm;

        private Toggle _togBag;
        private Toggle _togWeapon;
        private Button _btnLeft;
        private Button _btnRight;

        private bool _isBag = true;
        private List<StudentItem> _students;
        private int _curRoleIndex = 0;

        protected internal override void OnInit(object userData)
        {
            IsFullScreen = true;
            base.OnInit(userData);
            _bagForm = GetControl<UI_bag_form>("bag_form");
            _infoForm = GetControl<UI_info_form>("info_form");
            _hubForm = GetControl<UI_hub_form>("hub_form");
            _roleInfoForm = GetControl<UI_roleInfo_form>("roleInfo_form");
            _studentForm = GetControl<UI_student_form>("student_form");

            _togBag = GetControl<Toggle>("tog_bag");
            _togWeapon = GetControl<Toggle>("tog_weapon");
            _btnLeft = GetControl<Button>("btn_left");
            _btnRight = GetControl<Button>("btn_right");
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            _curRoleIndex = 0;
            _togBag.onValueChanged.RemoveListener(SwitchBagWeapon);
            _togWeapon.onValueChanged.RemoveListener(OnWeaponOpen);
            _btnLeft.onClick.RemoveListener(OnLeftBtnClick);
            _btnRight.onClick.RemoveListener(OnRightBtnClick);
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
            
            //因为是tog组，所以点另一个也会触发这一个，所以只用添加一个
            _togBag.onValueChanged.AddListener(SwitchBagWeapon);
            _togWeapon.onValueChanged.AddListener(OnWeaponOpen);
            _btnLeft.onClick.AddListener(OnLeftBtnClick);
            _btnRight.onClick.AddListener(OnRightBtnClick);

            _students = PlayerMgr.Instance.CurPlayer.Students.Values.ToList();

            //默认打开后为Bag面板
            _togBag.isOn = true;
            UpdateView(_isBag);
        }

        private void UpdateView(bool isBag)
        {
            if (isBag)
            {
                _infoForm.SetData(null);
                _hubForm.SetData(HideMe);
                _bagForm.SetData(0);
            }
            else
            {
                
            }
        }

        private void SwitchBagWeapon(bool isBag)
        {
            _isBag = isBag;
            bagPanel.SetActive(isBag);
            weaponPanel.SetActive(!isBag);
            UpdateView(isBag);
        }

        #region Student

        private void UpdateLrBtn()
        {
            _btnLeft.gameObject.SetActive(_curRoleIndex > 0);
            _btnRight.gameObject.SetActive(_curRoleIndex < _students.Count - 1);
        }

        private void OnLeftBtnClick()
        {
            ChangeRole(--_curRoleIndex);
            UpdateLrBtn();
        }

        private void OnRightBtnClick()
        {
            ChangeRole(++_curRoleIndex);
            UpdateLrBtn();
        }

        private void OnWeaponOpen(bool isOn)
        {
            if (isOn)
            {
                ChangeRole(_curRoleIndex);
                UpdateLrBtn();
            }
        }
        
        private void ChangeRole(int roleIndex)
        {
            _curRoleIndex = Mathf.Clamp(roleIndex, 0, _students.Count - 1);
            _roleInfoForm.SetData(_students[roleIndex]);
            _studentForm.SetData(_students[roleIndex]);
        }

        #endregion
    }
}