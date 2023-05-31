using Game.Managers;
using Game.Model;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public partial class UI_star_group : ControlBase
    {
        private const int STAR_WIDTH = 146;

        private StudentItem _student;

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            _student = null;
        }

        public void SetData(StudentItem student)
        {
            _student = student;
            img_star_old.rectTransform.sizeDelta =
                new Vector2(student.star * STAR_WIDTH, img_star_old.rectTransform.sizeDelta.y);
            img_star_new.rectTransform.sizeDelta =
                new Vector2(Mathf.Min(student.star + 1, StudentMgr.Instance.MaxStar) * STAR_WIDTH,
                    img_star_new.rectTransform.sizeDelta.y);
            
            //设置花费
            if (student.star == StudentMgr.Instance.MaxStar)
            {
                star_item_1.SetData(-1, 0);
                txt_cost.text = 0.ToString();
                btn_star.SetGray(true);
            }
            else
            {
                RoleStarInfo starInfo = StudentMgr.Instance.GetStarUpInfo(student.star);
                int pieceId = RoleMgr.Instance.GetRolePieceId(student.roleId);
                
                star_item_1.SetData(pieceId, starInfo.costPiece);
                
                bool notEnough = false;
                if (PlayerMgr.Instance.BagMaster.TryGetItem(0, 1, out var gold) &&
                    gold.num >= starInfo.cost)
                    txt_cost.text = starInfo.cost.ToString();
                else
                {
                    notEnough = true;
                    txt_cost.text = $"<color=red>{starInfo.cost}</color>";
                }
                if (PlayerMgr.Instance.BagMaster.TryGetItem(0, pieceId, out var item) &&
                    item.num >= starInfo.costPiece)
                    notEnough = true;
                btn_star.SetGray(notEnough);
                EventMgr.Instance.TriggerEvent(ClientEvent.STAR_PRE_UP);
            }
            
            btn_star.SetData("升星", OnStarUp);
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