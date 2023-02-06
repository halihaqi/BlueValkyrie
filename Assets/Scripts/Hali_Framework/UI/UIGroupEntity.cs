using UnityEngine;

namespace Hali_Framework
{
    public class UIGroupEntity : MonoBehaviour
    {
        private UIGroup _uiGroup;

        public void BindUIGroup(UIGroup group)
        {
            _uiGroup = group;
            group.UIGroupEntity = this;
        }

        public string Name => _uiGroup.Name;

        public int Depth => _uiGroup.Depth;

        public bool Pause => _uiGroup.Pause;

        public int PanelCount => _uiGroup.PanelCount;

        public string CurPanelName => _uiGroup.CurPanelEntity.name;

        public UIGroup UIGroup => _uiGroup;
    }
}