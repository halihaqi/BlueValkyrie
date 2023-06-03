using System;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public partial class UI_tip_btn : ControlBase
    {
        [SerializeField] private string key;
        [SerializeField] private string tip;
        private event Action Cb;
        
        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            EventMgr.Instance.RemoveListener(ClientEvent.GET_KEY_UP, Cb);
            Cb = null;
        }

        public void SetData(KeyCode key, string tip, Action callback)
        {
            if(Cb != null)
                EventMgr.Instance.RemoveListener(ClientEvent.GET_KEY_UP, Cb);
            Cb = callback;
            txt_key.text = key.ToString();
            txt_tip.text = tip;
            EventMgr.Instance.AddListener(ClientEvent.GET_KEY_UP, Cb);
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            transform.Find("txt_key").GetComponent<Text>().text = key;
            transform.Find("txt_tip").GetComponent<Text>().text = tip;
        }
    }
}