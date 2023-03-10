using System;
using DG.Tweening;
using Hali_Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI.Utils
{
    [RequireComponent(typeof(Selectable))]
    public class SelectableScaleTween : MonoBehaviour
    {
        public float toScale = 1.5f;
        public float duration = 1;
        
        private Selectable _control;
        private Vector3 _oriSize;
        
        private void Awake()
        {
            _control = GetComponent<Selectable>();
            _oriSize = transform.localScale;
        }

        private void OnEnable()
        {
            UIMgr.AddCustomEventListener(_control, EventTriggerType.PointerEnter, OnPointEnter);
            UIMgr.AddCustomEventListener(_control, EventTriggerType.PointerExit, OnPointExit);
        }

        private void OnDisable()
        {
            UIMgr.RemoveCustomEventListener(_control, EventTriggerType.PointerEnter, OnPointEnter);
            UIMgr.RemoveCustomEventListener(_control, EventTriggerType.PointerExit, OnPointExit);
        }

        private void OnPointEnter(BaseEventData data)
        {
            transform.DOScale(_oriSize * toScale, duration);
        }

        private void OnPointExit(BaseEventData data)
        {
            transform.DOScale(_oriSize, duration);
        }
    }
}