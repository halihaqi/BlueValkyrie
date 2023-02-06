namespace Hali_Framework
{
    public class PanelInfo : IReference
    {
        private PanelEntity _panelEntity;
        private bool _paused;
        private bool _covered;

        public PanelInfo()
        {
            _panelEntity = null;
            _paused = false;
            _covered = false;
        }

        public bool Paused
        {
            get => _paused;
            set => _paused = value;
        }

        public bool Covered
        {
            get => _covered;
            set => _covered = value;
        }

        public PanelEntity PanelEntity => _panelEntity;

        public static PanelInfo Create(PanelEntity panelEntity, bool isPaused = true, bool isCovered = true)
        {
            PanelInfo info = ReferencePoolMgr.Instance.PopRef<PanelInfo>();
            info._panelEntity = panelEntity;
            info._paused = isPaused;
            info._covered = isCovered;
            return info;
        }
        
        public void Reset()
        {
            _panelEntity = null;
            _paused = false;
            _covered = false;
        }
    }
}