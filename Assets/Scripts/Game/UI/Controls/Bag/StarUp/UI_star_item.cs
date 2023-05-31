using Game.Managers;
using Hali_Framework;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public partial class UI_star_item : ControlBase
    {
        private UI_normal_item _normalItem;
        private Image _imgEmpty;
        private Text _txtNum;

        protected internal override void OnInit()
        {
            base.OnInit();
            _normalItem = GetControl<UI_normal_item>("normal_item");
            _imgEmpty = GetControl<Image>("img_empty");
            _txtNum = GetControl<Text>("txt_num");
        }

        public void SetData(int itemId, int maxNum)
        {
            if (itemId == -1)
            {
                _imgEmpty.gameObject.SetActive(true);
                return;
            }
            
            _imgEmpty.gameObject.SetActive(false);
            _normalItem.SetData(itemId);
            int num = 0;
            if (PlayerMgr.Instance.BagMaster.TryGetItem(0, itemId, out var item))
                num = item.num;
            string str = num < maxNum ? $"<color=red>{num}</color>" : num.ToString();
            _txtNum.text = $"{str}/{maxNum}";
        }
    }
}