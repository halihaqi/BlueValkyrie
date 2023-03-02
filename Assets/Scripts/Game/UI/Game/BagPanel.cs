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
        private Text _txtGold;
        private Text _txtGem;
        
        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);
            _bagForm = GetControl<UI_bag_form>("bag_form");
            _infoForm = GetControl<UI_info_form>("info_form");
            _txtGem = GetControl<Text>("txt_gem");
            _txtGold = GetControl<Text>("txt_gold");
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
            if (!(userData is int bagId)) return;
            _bagId = bagId;
            
            UpdateView();
        }

        protected override void OnClick(string btnName)
        {
            base.OnClick(btnName);
            if (btnName == "btn_back")
                HideMe();
        }

        private void UpdateView()
        {
            _bagForm.SetData(_bagId);
            _infoForm.SetData(null);
            
            var bagMaster = PlayerMgr.Instance.BagMaster;
            _txtGold.text = bagMaster.TryGetItem(_bagId, 1, out var gold) ? gold.num.ToString() : "";
            _txtGem.text = bagMaster.TryGetItem(_bagId, 2, out var gem) ? gem.num.ToString() : "";
        }
    }
}