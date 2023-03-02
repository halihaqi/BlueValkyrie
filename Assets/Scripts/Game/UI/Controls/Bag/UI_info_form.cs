using Game.Managers;
using Game.Model.BagModel;
using Game.Utils;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public class UI_info_form : ControlBase
    {
        private Image _imgItem;
        private Image _imgAttributeGroup;
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
            _imgAttributeGroup = GetControl<Image>("img_attribute_group");
            _txtInfo = GetControl<Text>("txt_info");
            _txtNum = GetControl<Text>("txt_num");
            _txtAttribute = GetControl<Text>("txt_attribute");
            _txtAttributeNum = GetControl<Text>("txt_attribute_num");
            
            EventMgr.Instance.AddListener<ItemInfo, int>(ClientEvent.BAG_ITEM_CLICK, OnItemClick);
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            EventMgr.Instance.RemoveListener<ItemInfo, int>(ClientEvent.BAG_ITEM_CLICK, OnItemClick);
        }

        public void SetData(BagItemInfo bagInfo)
        {
            if (bagInfo == null)
            {
                this.gameObject.SetActive(false);
                return;
            }
            _info = BinaryDataMgr.Instance.GetInfo<ItemInfoContainer, int, ItemInfo>(bagInfo.id);
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
                var equip = EquipMgr.Instance.GetEquipInfo(_info.id);
                if (equip != null)
                {
                    _imgAttributeGroup.gameObject.SetActive(true);
                    _txtAttribute.text = EquipMgr.Instance.GetAttributeName(equip);
                    _txtAttributeNum.text = equip.attribute.ToString();
                }
                else
                    _imgAttributeGroup.gameObject.SetActive(false);
            }
            else
                _imgAttributeGroup.gameObject.SetActive(false);
        }

        private void OnItemClick(ItemInfo info, int num)
        {
            _info = info;
            _num = num;
            UpdateView();
        }
    }
}