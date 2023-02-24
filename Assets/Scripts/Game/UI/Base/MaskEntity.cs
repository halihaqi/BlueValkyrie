using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Base
{
    public class MaskEntity : MonoBehaviour
    {
        private Image _mask;
        private CanvasEntity _canvas;
        private CanvasGroup _canvasGroup;

        public Image Mask => _mask;

        private void Awake()
        {
            _mask = GetComponent<Image>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void SetAlpha(float alpha)
            => _canvasGroup.alpha = alpha;

        public void SetActive(bool isActive)
            => gameObject.SetActive(isActive);
    }
}