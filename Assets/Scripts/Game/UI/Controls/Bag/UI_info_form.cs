using Game.Managers;
using Game.Model.BagModel;
using Game.UI.Base;
using Game.Utils;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public class UI_info_form : ControlBase
    {
        private Image _imgItem;
        private ControlGroup _attributeGroup;
        private Text _txtInfo;
        private Text _txtNum;
        private Text _txtAttribute;
        private Text _txtAttributeNum;

        private ItemInfo _info;
        private int _num;
        
        protected internal override void OnInit()
        {
            base.OnInit();
            _imgItem = GetControl<Image>("img_item");
            _attributeGroup = GetControl<ControlGroup>("attribute_group");
            _txtInfo = GetControl<Text>("txt_info");
            _txtNum = GetControl<Text>("txt_num");
            _txtAttribute = GetControl<Text>("txt_attribute");
            _txtAttributeNum = GetControl<Text>("txt_attribute_num");
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            EventMgr.Instance.RemoveListener<ItemInfo, int>(ClientEvent.BAG_ITEM_CLICK, OnItemClick);
        }

        public void SetData(BagItemInfo bagInfo)
        {
            EventMgr.Instance.AddListener<ItemInfo, int>(ClientEvent.BAG_ITEM_CLICK, OnItemClick);
            if (bagInfo == null)
            {
                this.gameObject.SetActive(false);
                return;
            }
            
            _info = ItemMgr.Instance.GetItem(bagInfo.id);
            _num = bagInfo.num;

            UpdateView();
        }

        private void UpdateView()
        {
            this.gameObject.SetActive(true);
            ResMgr.Instance.LoadAsync<Sprite>(GameConst.RES_GROUP_UI, ResPath.GetItemIcon(_info), img =>
            {
                _imgItem.sprite = img;
            });
            _txtInfo.text = _info.fullName;
            _txtNum.text = _num.ToString();
            //装备才有属性
            if (_info.type == 1)
            {
                var equip = EquipMgr.Instance.FindEquip(_info.id);
                if (equip != null)
                {
                    _attributeGroup.SetActive(true);
                    _txtAttribute.text = EquipMgr.GetAttributeName(equip.type);
                    _txtAttributeNum.text = equip.attribute.ToString();
                }
                else
                    _attributeGroup.SetActive(false);
            }
            else
                _attributeGroup.SetActive(false);
        }

        private void OnItemClick(ItemInfo info, int num)
        {
            _info = info;
            _num = num;
            UpdateView();
        }
    }
}