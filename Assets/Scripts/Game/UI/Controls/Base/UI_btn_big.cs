using Hali_Framework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public class UI_btn_big : ControlBase
    {
        private Button _btn;
        private Image _imgAlpha;
        private Text _txtName;

        private Color _oriColor;
        private Color _oriTxtColor;
        
        protected internal override void OnInit()
        {
            base.OnInit();
            _btn = GetComponent<Button>();
            _imgAlpha = GetControl<Image>("img_alpha");
            _txtName = GetControl<Text>("txt_name");
            _oriColor = _imgAlpha.color;
            _oriTxtColor = _txtName.color;
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            _btn.onClick.RemoveAllListeners();
        }

        public void SetData(string btnName)
        {
            _txtName.text = btnName;
        }

        public void SetData(string btnName, UnityAction onClick)
        {
            _txtName.text = btnName;
            _btn.onClick.RemoveListener(onClick);
            _btn.onClick.AddListener(onClick);
            SetGray(false);
        }

        public void SetGray(bool isGray)
        {
            _btn.interactable = !isGray;
            _imgAlpha.color = !isGray ? _oriColor : Color.grey;
            _txtName.color = !isGray ? _oriTxtColor : Color.white;
        }
    }
}