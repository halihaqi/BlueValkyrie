using Game.Managers;
using Game.Model;
using Game.UI.Game;
using Hali_Framework;

namespace Game.UI.Controls
{
    public partial class UI_equip_group : ControlBase
    {
        private StudentItem _student;

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            EventMgr.Instance.RemoveListener(ClientEvent.WEAR_EQUIP, UpdateView);
        }

        public void SetData(StudentItem student)
        {
            equip_hat.SetClickListener(OnHatClick);
            equip_bag.SetClickListener(OnBagClick);
            equip_gloves.SetClickListener(OnGlovesClick);
            equip_shoes.SetClickListener(OnShoesClick);
            EventMgr.Instance.AddListener(ClientEvent.WEAR_EQUIP, UpdateView);
            
            _student = student;
            UpdateView();
        }

        private void UpdateView()
        {
            equip_hat.SetData(_student.equips[0]);
            equip_gloves.SetData(_student.equips[1]);
            equip_bag.SetData(_student.equips[2]);
            equip_shoes.SetData(_student.equips[3]);
        }

        private void OnHatClick()
        {
            UIMgr.Instance.ShowPanel<EquipPop>(GameConst.UIGROUP_POP,
                userData: new EquipParam(EquipType.Hat, _student));
        }
        private void OnGlovesClick()
        {
            UIMgr.Instance.ShowPanel<EquipPop>(GameConst.UIGROUP_POP,
                userData: new EquipParam(EquipType.Gloves, _student));
        }
        private void OnBagClick()
        {
            UIMgr.Instance.ShowPanel<EquipPop>(GameConst.UIGROUP_POP,
                userData: new EquipParam(EquipType.Bag, _student));
        }
        private void OnShoesClick()
        {
            UIMgr.Instance.ShowPanel<EquipPop>(GameConst.UIGROUP_POP,
                userData: new EquipParam(EquipType.Shoes, _student));
        }
    }
}