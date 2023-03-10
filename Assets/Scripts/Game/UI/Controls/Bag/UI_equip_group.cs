using Game.Managers;
using Game.Model;
using Game.UI.Game;
using Hali_Framework;

namespace Game.UI.Controls
{
    public class UI_equip_group : ControlBase
    {
        private UI_equip_item _equipHat;
        private UI_equip_item _equipBag;
        private UI_equip_item _equipGloves;
        private UI_equip_item _equipShoes;
        private StudentItem _student;

        protected internal override void OnInit()
        {
            base.OnInit();
            _equipHat = GetControl<UI_equip_item>("equip_hat");
            _equipBag = GetControl<UI_equip_item>("equip_bag");
            _equipGloves = GetControl<UI_equip_item>("equip_gloves");
            _equipShoes = GetControl<UI_equip_item>("equip_shoes");
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            EventMgr.Instance.RemoveListener(ClientEvent.WEAR_EQUIP, UpdateView);
        }

        public void SetData(StudentItem student)
        {
            _equipHat.SetClickListener(OnHatClick);
            _equipBag.SetClickListener(OnBagClick);
            _equipGloves.SetClickListener(OnGlovesClick);
            _equipShoes.SetClickListener(OnShoesClick);
            EventMgr.Instance.AddListener(ClientEvent.WEAR_EQUIP, UpdateView);
            
            _student = student;
            UpdateView();
        }

        private void UpdateView()
        {
            _equipHat.SetData(_student.equips[0]);
            _equipGloves.SetData(_student.equips[1]);
            _equipBag.SetData(_student.equips[2]);
            _equipShoes.SetData(_student.equips[3]);
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