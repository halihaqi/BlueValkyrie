using Game.Managers;
using Game.UI.Controls;
using Hali_Framework;
using UnityEngine.UI;

namespace Game.UI.Game
{
    public class BagPanel : PanelBase
    {
        private int _bagId = -1;
        private UI_bag_form _bagForm;
        private UI_info_form _infoForm;
        private UI_hub_form _hubForm;

        protected internal override void OnInit(object userData)
        {
            IsFullScreen = true;
            base.OnInit(userData);
            _bagForm = GetControl<UI_bag_form>("bag_form");
            _infoForm = GetControl<UI_info_form>("info_form");
            _hubForm = GetControl<UI_hub_form>("hub_form");
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
            if (!(userData is int bagId)) return;
            _bagId = bagId;
            
            UpdateView();
        }

        private void UpdateView()
        {
            _infoForm.SetData(null);
            _hubForm.SetData(HideMe);
            _bagForm.SetData(_bagId);
        }
    }
}