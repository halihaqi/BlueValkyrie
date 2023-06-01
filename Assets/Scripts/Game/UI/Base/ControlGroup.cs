using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI.Base
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ControlGroup : UIBehaviour
    {
        private CanvasGroup _canvasGroup;

        public float Alpha => _canvasGroup.alpha;

        protected override void Awake()
        {
            base.Awake();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        public void SetAlpha(float alpha)
        {
            _canvasGroup.alpha = alpha;
        }

        public void SetBlocksRaycasts(bool isEnable)
        {
            _canvasGroup.blocksRaycasts = isEnable;
        }
    }
}