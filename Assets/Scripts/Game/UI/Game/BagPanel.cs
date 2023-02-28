using Game.UI.Controls;
using Hali_Framework;
using UnityEngine.UI;

namespace Game.UI.Game
{
    public class BagPanel : PanelBase
    {
        private UI_bag_form _bagForm;
        private Button _btnBack;
        
        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);
            _bagForm = GetControl<UI_bag_form>("bag_form");
            _btnBack = GetControl<Button>("btn_back");
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
            if(userData is int bagId)
                _bagForm.SetData(bagId);
        }
    }
}