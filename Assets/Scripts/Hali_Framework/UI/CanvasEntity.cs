using System;
using UnityEngine;
using UnityEngine.UI;

namespace Hali_Framework
{
    public class CanvasEntity : MonoBehaviour
    {
        private Canvas _canvas;
        private CanvasScaler _canvasScaler;
        
        //Canvas
        public RenderMode renderMode = RenderMode.ScreenSpaceOverlay;
        public int sortOrder = 0;
        
        //CanvasScaler
        public CanvasScaler.ScaleMode scaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        public Vector2 referenceResolution = new Vector2(1920, 1080);
        public CanvasScaler.ScreenMatchMode screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        [Range(0, 1)] public float matchWidthOrHeight = 1;

        private void Awake()
        {
            DontDestroyOnLoad(this);
            if(!gameObject.TryGetComponent(out _canvas))
                _canvas = gameObject.AddComponent<Canvas>();
            if(!gameObject.TryGetComponent(out _canvasScaler))
                _canvasScaler = gameObject.AddComponent<CanvasScaler>();
            if(gameObject.GetComponent<GraphicRaycaster>() == null)
                gameObject.AddComponent<GraphicRaycaster>();
            UpdateConfig();
        }

        public void UpdateConfig()
        {
            //Canvas
            _canvas.renderMode = renderMode;
            _canvas.sortingOrder = sortOrder;
            
            //CanvasScaler
            _canvasScaler.uiScaleMode = scaleMode;
            _canvasScaler.referenceResolution = referenceResolution;
            _canvasScaler.screenMatchMode = screenMatchMode;
            _canvasScaler.matchWidthOrHeight = matchWidthOrHeight;
        }
    }
}