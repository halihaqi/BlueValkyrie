using Hali_Framework;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public partial class UI_reward_item : ControlBase
    {
        public void SetData(int itemId, int num, bool rare)
        {
            img_rare.gameObject.SetActive(rare);
            normal_item.SetData(itemId, num);
        }
    }
}