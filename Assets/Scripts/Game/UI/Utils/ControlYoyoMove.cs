using System;
using DG.Tweening;
using UnityEngine;

namespace Game.UI.Utils
{
    public class ControlYoyoMove : MonoBehaviour
    {
        public float duration = 1;//持续时间
        public Vector2 moveOffset = Vector2.right;//目标偏移

        private RectTransform _rect;

        private void Awake()
        {
            _rect = transform as RectTransform;
        }

        private void OnEnable()
        {
            _rect.DOAnchorPos(_rect.anchoredPosition + moveOffset, duration)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void OnDisable()
        {
            _rect.DOKill();
        }
    }
}