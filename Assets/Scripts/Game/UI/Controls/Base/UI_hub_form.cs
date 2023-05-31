using Game.Managers;
using Hali_Framework;
using UnityEngine.Events;

namespace Game.UI.Controls
{
    public partial class UI_hub_form : ControlBase
    {
        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            btn_back.onClick.RemoveAllListeners();
            EventMgr.Instance.RemoveListener(ClientEvent.EXP_UP, UpdateView);
            EventMgr.Instance.RemoveListener(ClientEvent.STAR_UP, UpdateView);

        }

        public void SetData(UnityAction onClick)
        {
            EventMgr.Instance.AddListener(ClientEvent.EXP_UP, UpdateView);
            EventMgr.Instance.AddListener(ClientEvent.STAR_UP, UpdateView);

            btn_back.onClick.AddListener(onClick);
            UpdateView();
        }

        public void UpdateView()
        {
            var bagMaster = PlayerMgr.Instance.BagMaster;
            txt_gold.text = bagMaster.TryGetItem
                (0, 1, out var gold) ? gold.num.ToString() : 0.ToString();
            txt_gem.text = bagMaster.TryGetItem
                (0, 2, out var gem) ? gem.num.ToString() : 0.ToString();
        }
    }
}