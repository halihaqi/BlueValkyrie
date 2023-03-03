using UnityEngine;

namespace Hali_Framework
{
    public class InputMgr : Singleton<InputMgr>, IModule
    {
        //是否开启输入检测
        private bool _enabled = false;
        public int Priority => 0;

        private Vector2 _inputMouseDelta;
        private Vector2 _inputMoveDelta;

        void IModule.Update(float elapseSeconds, float realElapseSeconds)
        {
            InputUpdate();
        }

        void IModule.Dispose()
        {
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
            if (!_enabled) return;
            
            KeyCheck(KeyCode.B);
            KeyCheck(KeyCode.Q);
            KeyCheck(KeyCode.E);
            KeyCheck(KeyCode.Tab);
            KeyCheck(KeyCode.Escape);
            
            _inputMouseDelta.x = Input.GetAxis("Mouse X");
            _inputMouseDelta.y = Input.GetAxis("Mouse Y");
            _inputMoveDelta.x = Input.GetAxis("Horizontal");
            _inputMoveDelta.y = Input.GetAxis("Vertical");
        }


        #region 外部调用方法
        
        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                if (!_enabled)
                {
                    _inputMoveDelta = default;
                    _inputMouseDelta = default;
                }
            }
        }

        public Vector2 GetInputMove => _inputMoveDelta;

        public Vector2 GetInputLook => _inputMouseDelta;

        #endregion
    }
}
