using System;
using UnityEngine;

namespace Hali_Framework
{
    [RequireComponent(typeof(Camera))]
    public class UICameraEntity : MonoBehaviour
    {
        private Camera _cam;

        public Camera Camera => _cam;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            _cam = GetComponent<Camera>();
            var uiLayer = LayerMask.NameToLayer("UI");
            _cam.gameObject.layer = uiLayer;
            _cam.cullingMask = 1 << uiLayer;
            _cam.clearFlags = CameraClearFlags.Depth;
            _cam.depth = 10;
            _cam.transform.position = Vector3.up * 1000f;//移至上方1000米
        }
    }
}