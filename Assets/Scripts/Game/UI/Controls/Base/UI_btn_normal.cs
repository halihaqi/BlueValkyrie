using Hali_Framework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public partial class UI_btn_normal : ControlBase
    {
        private Button _btn;
        private Color _oriColor;
        
        protected internal override void OnInit()
        {
            base.OnInit();
            _btn = GetComponent<Button>();
            txt_name = GetControl<Text>("txt_name");
            _oriColor = txt_name.color;
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
        }

        public void SetGray(bool isGray)
        {
            _btn.interactable = !isGray;
            txt_name.color = !isGray ? _oriColor : Color.grey;
        }
    }
}