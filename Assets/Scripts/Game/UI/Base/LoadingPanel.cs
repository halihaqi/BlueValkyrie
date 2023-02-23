using Hali_Framework;
using UnityEngine.UI;

namespace Game.UI.Base
{
    public class LoadingPanel : PanelBase
    {
        private string _lastPoint;
        private Text _txtPoint;
        private Slider _sldLoad;
        
        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);
            _txtPoint = GetControl<Text>("txt_point");
            _sldLoad = GetControl<Slider>("sld_load");
            _sldLoad.maxValue = 100;
            _sldLoad.minValue = 0;
            _sldLoad.value = 0;
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
            _txtPoint.text = _lastPoint = "";
            _sldLoad.value = 0;
            DelayUtils.Instance.Loop(0.3f, LoopPoint);
            EventMgr.Instance.AddListener<int>(ClientEvent.LOADING, OnLoading);
        }

        protected internal override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            DelayUtils.Instance.Remove(LoopPoint);
            EventMgr.Instance.RemoveListener<int>(ClientEvent.LOADING, OnLoading);
        }

        private void OnLoading(int progress)
        {
            _sldLoad.value = progress;
        }

        private void LoopPoint(object obj)
        {
            if (_lastPoint.Length < 3)
                _lastPoint += ".";
            else
                _lastPoint = "";
            _txtPoint.text = _lastPoint;
        }
    }
}