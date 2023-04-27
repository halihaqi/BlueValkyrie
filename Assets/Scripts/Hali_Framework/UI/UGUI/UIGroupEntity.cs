using System;
using UnityEngine;

namespace Hali_Framework
{
    [RequireComponent(typeof(RectTransform))]
    public class UIGroupEntity : MonoBehaviour
    {
        private UIGroup _uiGroup;

        public string Name => _uiGroup.Name;

        public int Depth => _uiGroup.Depth;

        public bool Pause => _uiGroup.Pause;

        public int PanelCount => _uiGroup.PanelCount;

        public string CurPanelName => _uiGroup.CurPanelEntity.name;

        public UIGroup UIGroup => _uiGroup;
        

        private void Awake()
        {
            var rect = (RectTransform)transform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            gameObject.layer = LayerMask.NameToLayer("UI");
        }
        
        public void BindUIGroup(UIGroup group)
        {
            _uiGroup = group;
            group.UIGroupEntity = this;
        }
    }
}