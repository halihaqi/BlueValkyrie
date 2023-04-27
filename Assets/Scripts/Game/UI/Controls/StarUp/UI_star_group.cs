using Game.Managers;
using Game.Model;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Controls.StarUp
{
    public partial class UI_star_group : ControlBase
    {
        private const int STAR_WIDTH = 146;

        private UI_star_item _starItem1;
        private UI_btn_big _btnStar;
        private Text _txtCost;
        private Image _imgStarOld;
        private Image _imgStarNew;

        private StudentItem _student;

        protected internal override void OnInit()
        {
            base.OnInit();
            _starItem1 = GetControl<UI_star_item>("star_item_1");
            _btnStar = GetControl<UI_btn_big>("btn_star");
            _txtCost = GetControl<Text>("txt_cost");
            _imgStarNew = GetControl<Image>("img_star_new");
            _imgStarOld = GetControl<Image>("img_star_old");
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            _student = null;
        }

        public void SetData(StudentItem student)
        {
            _student = student;
            _imgStarOld.rectTransform.sizeDelta =
                new Vector2(student.star * STAR_WIDTH, _imgStarOld.rectTransform.sizeDelta.y);
            _imgStarNew.rectTransform.sizeDelta =
                new Vector2(Mathf.Min(student.star + 1, StudentMgr.Instance.MaxStar) * STAR_WIDTH,
                    _imgStarNew.rectTransform.sizeDelta.y);
            
            //设置花费
            if (student.star == StudentMgr.Instance.MaxStar)
            {
                _starItem1.SetData(-1, 0);
                _txtCost.text = 0.ToString();
                _btnStar.SetGray(true);
            }
            else
            {
                RoleStarInfo starInfo = StudentMgr.Instance.GetStarUpInfo(student.star);
                int pieceId = RoleMgr.Instance.GetRolePieceId(student.roleId);
                
                _starItem1.SetData(pieceId, starInfo.costPiece);
                
                bool notEnough = false;
                if (PlayerMgr.Instance.BagMaster.TryGetItem(0, 1, out var gold) &&
                    gold.num >= starInfo.cost)
                    _txtCost.text = starInfo.cost.ToString();
                else
                {
                    notEnough = true;
                    _txtCost.text = $"<color=red>{starInfo.cost}</color>";
                }
                if (PlayerMgr.Instance.BagMaster.TryGetItem(0, pieceId, out var item) &&
                    item.num >= starInfo.costPiece)
                    notEnough = true;
                _btnStar.SetGray(notEnough);
                EventMgr.Instance.TriggerEvent(ClientEvent.STAR_PRE_UP);
            }
            
            _btnStar.SetData("升星", OnStarUp);
        }

        private void OnStarUp()
        {
            TipHelper.ShowConfirm("真的要升星吗？", () =>
            {
                RoleStarInfo starInfo = StudentMgr.Instance.GetStarUpInfo(_student.star);
                var bag = PlayerMgr.Instance.BagMaster;
                //减材料
                bag.AddItem(0, RoleMgr.Instance.GetRolePieceId(_student.roleId), -starInfo.costPiece);
                bag.AddItem(0, 1, -starInfo.cost);
                //升星
                _student.StarUp();
                SetData(_student);
                TipHelper.ShowTip("升星成功!");
                EventMgr.Instance.TriggerEvent(ClientEvent.STAR_UP);
            });
        }
    }
}