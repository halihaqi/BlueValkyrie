using Hali_Framework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public partial class UI_btn_normal : ControlBase
    {
        private Button _btn;
        private Text _txtName;
        private Color _oriColor;
        
        protected internal override void OnInit()
        {
            base.OnInit();
            _btn = GetComponent<Button>();
            _txtName = GetControl<Text>("txt_name");
            _oriColor = _txtName.color;
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
        }

        public void SetGray(bool isGray)
        {
            _btn.interactable = !isGray;
            _txtName.color = !isGray ? _oriColor : Color.grey;
        }
    }
}