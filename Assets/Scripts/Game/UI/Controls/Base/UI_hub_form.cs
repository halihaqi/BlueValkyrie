using Game.Managers;
using Hali_Framework;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public class UI_hub_form : ControlBase
    {
        private Text _txtGold;
        private Text _txtGem;
        private Button _btnBack;
        
        protected internal override void OnInit()
        {
            base.OnInit();
            _txtGem = GetControl<Text>("txt_gem");
            _txtGold = GetControl<Text>("txt_gold");
            _btnBack = GetControl<Button>("btn_back");
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            _btnBack.onClick.RemoveAllListeners();
        }

        public void SetData(UnityAction onClick)
        {
            _btnBack.onClick.RemoveListener(onClick);
            _btnBack.onClick.AddListener(onClick);
            UpdateView();
        }

        public void UpdateView()
        {
            var bagMaster = PlayerMgr.Instance.BagMaster;
            _txtGold.text = bagMaster.TryGetItem
                (0, 1, out var gold) ? gold.num.ToString() : 0.ToString();
            _txtGem.text = bagMaster.TryGetItem
                (0, 2, out var gem) ? gem.num.ToString() : 0.ToString();
        }
    }
}