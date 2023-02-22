using System;
using Hali_Framework;
using UnityEngine.UI;

namespace Game.UI.Base
{
    public class InputPop : PopBase
    {
        private InputField _ifName;
        private Action<string> _callback;

        protected internal override void OnInit(object userData)
        {
            isModal = true;
            base.OnInit(userData);
            _ifName = GetControl<InputField>("if_name");
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
            if (userData is Action<string> act)
                _callback = act;
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            _ifName.text = "";
        }

        protected override void OnClick(string btnName)
        {
            base.OnClick(btnName);
            if (btnName.Equals("btn_sure"))
            {
                if (string.IsNullOrEmpty(_ifName.text))
                {
                    //tip
                    return;
                }
                _callback?.Invoke(_ifName.text);
                HideMe();
            }
        }
    }
}