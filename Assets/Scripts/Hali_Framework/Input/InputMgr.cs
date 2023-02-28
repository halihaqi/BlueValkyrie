using UnityEngine;

namespace Hali_Framework
{
    public class InputMgr : Singleton<InputMgr>, IModule
    {
        //是否开启输入检测
        private bool _isOpenCheck = false;
        public int Priority => 0;

        void IModule.Update(float elapseSeconds, float realElapseSeconds)
        {
            InputUpdate();
        }

        void IModule.Dispose()
        {
            MonoMgr.Instance.RemoveUpdateListener(InputUpdate);
        }

        /// <summary>
        /// 检查Key输入
        /// </summary>
        /// <param name="key"></param>
        private void KeyCheck(KeyCode key)
        {
            if (Input.GetKeyDown(key))
                EventMgr.Instance.TriggerEvent(ClientEvent.GET_KEY_DOWN, key);
            if(Input.GetKeyUp(key))
                EventMgr.Instance.TriggerEvent(ClientEvent.GET_KEY_UP, key);
            if(Input.GetKey(key))
                EventMgr.Instance.TriggerEvent(ClientEvent.GET_KEY, key);
        }

        /// <summary>
        /// 在GlobalMono每帧调用
        /// </summary>
        private void InputUpdate()
        {
            if (!_isOpenCheck)
                return;
            KeyCheck(KeyCode.B);
            KeyCheck(KeyCode.Q);
            KeyCheck(KeyCode.E);
            KeyCheck(KeyCode.Tab);
            KeyCheck(KeyCode.Escape);
        }


        #region 外部调用方法
        /// <summary>
        /// 开启关闭输入检测
        /// </summary>
        /// <param name="isOpen"></param>
        public void OpenOrClose(bool isOpen)
        {
            _isOpenCheck = isOpen;
        }

        #endregion
    }
}
