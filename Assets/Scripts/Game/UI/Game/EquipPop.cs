using System.Collections.Generic;
using System.Linq;
using Game.Managers;
using Game.Model;
using Game.Model.BagModel;
using Game.UI.Controls;
using Hali_Framework;

namespace Game.UI.Game
{
    public class EquipPop : PopBase
    {
        private UI_bag_form _bagForm;
        private StudentItem _student;
        private EquipType _equipType;

        protected internal override void OnInit(object userData)
        {
            isModal = true;
            base.OnInit(userData);
            _bagForm = GetControl<UI_bag_form>("bag_form");
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            _student = null;
            EventMgr.Instance.RemoveListener<ItemInfo, int>(ClientEvent.BAG_ITEM_CLICK, OnClickItem);
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
            EventMgr.Instance.AddListener<ItemInfo, int>(ClientEvent.BAG_ITEM_CLICK, OnClickItem);
            if (userData is EquipParam p)
            {
                _equipType = p.equipType;
                _student = p.student;
            }
            
            _bagForm.SetData(FindTypeEquips());
        }

        private List<BagItemInfo> FindTypeEquips()
        {
            var equips = PlayerMgr.Instance.BagMaster.GetItemsByType(0, ItemType.Equip);
            return (from item in equips
                    let equip = EquipMgr.Instance.FindEquip(item.id)
                    where equip.type == (int)_equipType
                    select item).ToList();
        }

        private void OnClickItem(ItemInfo item, int itemNum)
        {
            var oldEquip = _student.equips[(int)_equipType];
            var newEquip = EquipMgr.Instance.FindEquip(item.id);
            //如果新穿装备
            if (oldEquip == null)
            {
                TipHelper.ShowConfirm("是否穿戴该装备？", () =>
                {
                    _student.equips[(int)_equipType] = newEquip;
                    PlayerMgr.Instance.BagMaster.AddItem(0, item, -1);
                    TipHelper.ShowTip("装备成功!");
                    //刷新UI
                    _bagForm.SetData(FindTypeEquips());
                    EventMgr.Instance.TriggerEvent(ClientEvent.WEAR_EQUIP);
                    HideMe();
                });
            }
            else
            {
                TipHelper.ShowConfirm("是否替换该装备？", () =>
                {
                    _student.equips[(int)_equipType] = newEquip;
                    PlayerMgr.Instance.BagMaster.AddItem(0, item, -1);
                    PlayerMgr.Instance.BagMaster.AddItem(0, oldEquip.itemId, 1);
                    TipHelper.ShowTip("装备成功!");
                    //刷新UI
                    _bagForm.SetData(FindTypeEquips());
                    EventMgr.Instance.TriggerEvent(ClientEvent.WEAR_EQUIP);
                    HideMe();
                });
            }
        }
    }

    public class EquipParam
    {
        public EquipType equipType;
        public StudentItem student;

        public EquipParam(EquipType equipType, StudentItem student)
            => (this.equipType, this.student) = (equipType, student);
    }
}