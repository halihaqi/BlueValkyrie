using UnityEngine;
using UnityEngine.UI;

namespace Hali_Framework
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public class CanvasEntity : MonoBehaviour
    {
        private Canvas _canvas;
        private CanvasScaler _canvasScaler;
        
        //Canvas
        public RenderMode renderMode = RenderMode.ScreenSpaceCamera;
        public Camera renderCamera;
        public int sortOrder = 0;
        
        //CanvasScaler
        public CanvasScaler.ScaleMode scaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        public Vector2 referenceResolution = new Vector2(1920, 1080);
        public CanvasScaler.ScreenMatchMode screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        [Range(0, 1)] public float matchWidthOrHeight = 1;

        private void Awake()
        {
            DontDestroyOnLoad(this);
            _canvas = GetComponent<Canvas>();
            _canvasScaler = GetComponent<CanvasScaler>();
            _canvas.gameObject.layer = LayerMask.NameToLayer("UI");
            UpdateConfig();
        }

        public void UpdateConfig()
        {
            //Canvas
            _canvas.renderMode = renderMode;
            if (renderMode == RenderMode.ScreenSpaceCamera)
                _canvas.worldCamera = renderCamera;
            
            _canvas.sortingOrder = sortOrder;
            
            //CanvasScaler
            _canvasScaler.uiScaleMode = scaleMode;
            _canvasScaler.referenceResolution = referenceResolution;
            _canvasScaler.screenMatchMode = screenMatchMode;
            _canvasScaler.matchWidthOrHeight = matchWidthOrHeight;
        }
    }
}