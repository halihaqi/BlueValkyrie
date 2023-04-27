using Hali_Framework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public partial class UI_btn_big : ControlBase
    {
        private Button _btn;

        private Color _oriColor;
        private Color _oriTxtColor;
        
        protected internal override void OnInit()
        {
            base.OnInit();
            _btn = GetComponent<Button>();
            _oriColor = img_alpha.color;
            _oriTxtColor = txt_name.color;
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            _btn.onClick.RemoveAllListeners();
        }

        public void SetData(string btnName)
        {
            txt_name.text = btnName;
        }

        public void SetData(string btnName, UnityAction onClick)
        {
            txt_name.text = btnName;
            _btn.onClick.RemoveListener(onClick);
            _btn.onClick.AddListener(onClick);
            SetGray(false);
        }

        public void SetGray(bool isGray)
        {
            _btn.interactable = !isGray;
            img_alpha.color = !isGray ? _oriColor : Color.grey;
            txt_name.color = !isGray ? _oriTxtColor : Color.white;
        }
    }
}