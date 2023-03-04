using System.Collections.Generic;
using Game.Managers;
using Game.UI.Controls;
using Game.Utils;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Game
{
    public class ShopBuyPop : PopBase
    {
        //固定
        private UI_normal_item _normalItem;
        private Image _imgSingleCost;
        private Image _imgMultCost;
        private Text _txtItemName;
        private Text _txtSingleCost;
        
        //动态
        private Text _txtBuyNum;
        private Text _txtMultCost;

        private List<UI_btn_normal> _btnNormals;

        private ShopItemInfo _info;
        private int _num;
        private int _price;
        private int _curNum = 1;

        protected internal override void OnInit(object userData)
        {
            isModal = true;
            base.OnInit(userData);
            _imgSingleCost = GetControl<Image>("img_single_cost");
            _imgMultCost = GetControl<Image>("img_mult_cost");
            _normalItem = GetControl<UI_normal_item>("normal_item");

            _btnNormals = GetControls<UI_btn_normal>();

            _txtItemName = GetControl<Text>("txt_item_name");
            _txtBuyNum = GetControl<Text>("txt_buy_num");
            _txtSingleCost = GetControl<Text>("txt_single_cost");
            _txtMultCost = GetControl<Text>("txt_mult_cost");
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
            if (userData is ShopBuyParam p)
            {
                _info = p.info;
                _num = p.num;
                _price = _info.price;
            }

            _txtItemName.text = _info.name;
            _normalItem.SetData(_info.itemId);
            ResMgr.Instance.LoadAsync<Sprite>(GameConst.RES_GROUP_UI, ResPath.GetItemIcon(_info.currencyId), img =>
            {
                _imgSingleCost.sprite = img;
                _imgMultCost.sprite = img;
            });
            _txtSingleCost.text = _price.ToXNum();
            SetNum(1);
        }

        protected override void OnClick(string btnName)
        {
            base.OnClick(btnName);
            switch (btnName)
            {
                case "btn_back":
                case "btn_cancel":
                    HideMe();
                    break;
                case "btn_sub":
                    AddNum(-1);
                    break;
                case "btn_add":
                    AddNum(1);
                    break;
                case "btn_max":
                    SetNum(_num);
                    break;
                case "btn_min":
                    SetNum(1);
                    break;
                case "btn_buy":
                    if (!ShopMgr.Instance.Buy(_info.itemId, _curNum))
                    {
                        TipMgr.Instance.ShowTip("货币不够啦！");
                    }
                    else
                    {
                        TipMgr.Instance.ShowTip("购买成功！");
                        HideMe();
                    }
                    break;
            }
        }

        private void AddNum(int addNum)
        {
            _curNum += addNum;
            UpdateView();
        }

        private void SetNum(int num)
        {
            _curNum = num;
            UpdateView();
        }

        private void UpdateBtn()
        {
            bool isMax = _curNum >= _num;
            bool isMin = _curNum <= 1;
            foreach (var btn in _btnNormals)
            {
                switch (btn.name)
                {
                    case "btn_sub":
                    case "btn_min":
                        btn.SetGray(isMin);
                        break;
                    
                    case "btn_add":
                    case "btn_max":
                        btn.SetGray(isMax);
                        break;
                }
            }
        }

        private void UpdateView()
        {
            _txtBuyNum.text = _curNum.ToString();
            _txtMultCost.text = (_price * _curNum).ToXNum();
            UpdateBtn();
        }
    }

    public class ShopBuyParam
    {
        public ShopItemInfo info;
        public int num;

        public ShopBuyParam(ShopItemInfo info, int num) => (this.info, this.num) = (info, num);
    }
}