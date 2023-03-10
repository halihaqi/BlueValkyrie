using Hali_Framework;
using UnityEngine.UI;

namespace Game.UI.Controls.Episode
{
    public class UI_reward_item : ControlBase
    {
        private UI_normal_item _normalItem;
        private Image _imgRare;

        protected internal override void OnInit()
        {
            base.OnInit();
            _normalItem = GetControl<UI_normal_item>("normal_item");
            _imgRare = GetControl<Image>("img_rare");
        }

        public void SetData(int itemId, int num, bool rare)
        {
            _imgRare.gameObject.SetActive(rare);
            _normalItem.SetData(itemId, num);
        }
    }
}