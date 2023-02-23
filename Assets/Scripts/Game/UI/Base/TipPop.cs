using DG.Tweening;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Base
{
    public class TipPop : PopBase
    {
        private Image _imgBk;
        private Text _tip;
        private float _tipHeight;
        private const float DURATION = 0.3f;
        private const float RESIDENCE_TIME = 1f;
        private const float POP_PERCENT = 0.8f;
        
        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);
            _tip = GetControl<Text>("txt_tip");
            _imgBk = GetControl<Image>("img_bk");
            _tipHeight = _imgBk.rectTransform.rect.height;

            PanelEntity.SetCustomFade(DURATION + 0.1f);
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
            if (userData is string str)
                _tip.text = str;
            //先将tip置于画面外
            _imgBk.rectTransform.anchoredPosition = Vector2.up * _tipHeight;
            
            _imgBk.rectTransform.DOAnchorPos(_imgBk.rectTransform.anchoredPosition - Vector2.up * (_tipHeight * POP_PERCENT), DURATION);
            
            DelayUtils.Instance.Delay(DURATION + RESIDENCE_TIME, 1, DelayHideMe);
        }

        protected internal override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            _imgBk.rectTransform.DOAnchorPos(Vector2.up * _tipHeight, DURATION);
        }

        private void DelayHideMe(object obj) => HideMe();
    }
}