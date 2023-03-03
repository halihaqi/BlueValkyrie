using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Hali_Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Controls.Dialogue
{
    public class UI_dialogue_frame : ControlBase
    {
        [SerializeField]
        private string text;
        [SerializeField]
        private float offset = 160;
        [SerializeField]
        private float arrowOffset = 10;

        private RectTransform _rect;
        private RectTransform _arrow;
        private float _arrowOriY;
        private Sequence _arrowTween;
        private Text _txtDialogue;
        //为了逐字显示时文本框固定，用一个隐藏的文本框规定大小
        //再通过改变显示的文本框实现逐字显示
        private Text _txtHide;

        private float _interval = 0;
        private float _sentenceInterval = 1;
        private Queue<string> _textQueue;
        private bool _isShowing = false;
        private Dictionary<int, Action<string>> _perSentenceCallbacks;
        private Action<UI_dialogue_frame> _completeCallback;
        
        public bool IsShowing => _isShowing;

        protected internal override void OnInit()
        {
            base.OnInit();
            _txtDialogue = GetControl<Text>("txt_dialogue");
            _txtHide = GetControl<Text>("txt_hide");
            
            _arrow = GetControl<Image>("img_next").rectTransform;
            _rect = this.transform as RectTransform;

            _arrowOriY = _arrow.anchoredPosition.y;
            _arrowTween = DOTween.Sequence();
            _arrowTween.Append(_arrow.DOAnchorPosY(_arrowOriY + arrowOffset, 0.3f));
            _arrowTween.Append(_arrow.DOAnchorPosY(_arrowOriY, 1));
            _arrowTween.SetLoops(-1);
            _textQueue = new Queue<string>();
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            StopAllCoroutines();
            _arrowTween.Kill();
        }

        public void SetData(string str, float interval = 0, float sentenceInterval = 1)
        {
            if (_isShowing)
                Skip();
            text = str;
            _interval = interval;
            _sentenceInterval = sentenceInterval;

            StartCoroutine(VerbatimShowSentence());
        }

        public void SetData(string[] strs, float interval = 0, float sentenceInterval = 1)
        {
            if (_isShowing)
                Skip();
            _textQueue.Clear();
            foreach (var str in strs)
                _textQueue.Enqueue(str);
            _interval = interval;
            _sentenceInterval = sentenceInterval;
            
            StartCoroutine(VerbatimShowSentences());
        }

        public void SetData(Managers.Dialogue dialogue, float interval = 0.1f)
        {
            _perSentenceCallbacks = dialogue.callbacks;
            SetData(dialogue.text, interval, dialogue.interval);
        }

        
        public void AddCompleteEvent(Action<UI_dialogue_frame> callback)
        {
            _completeCallback -= callback;
            _completeCallback += callback;
        }
        
        public void RemoveCompleteEvent(Action<UI_dialogue_frame> callback)
        {
            _completeCallback -= callback;
        }

        public void Skip()
        {
            StopAllCoroutines();
            _txtDialogue.text = text;
            _isShowing = false;
            _completeCallback?.Invoke(this);
        }

        private IEnumerator VerbatimShowSentence()
        {
            _isShowing = true;
            //设置文本框大小
            _txtHide.text = text;
            _rect.sizeDelta = new Vector2(_txtHide.preferredWidth + offset, _rect.sizeDelta.y);

            _txtDialogue.text = "";
            for (int i = 0; i < text.Length; i++)
            {
                _txtDialogue.text += text[i];
                if (_interval > 0)
                    yield return new WaitForSecondsRealtime(_interval);
                else
                    yield return null;
            }
            
            //显示完再隔一个时间段结束
            if (_sentenceInterval > 0)
                yield return new WaitForSecondsRealtime(_sentenceInterval);
            else
                yield return null;
            
            _completeCallback?.Invoke(this);
            _isShowing = false;
        }
        
        private IEnumerator VerbatimShowSentences()
        {
            _isShowing = true;
            int index = 0;
            while (_textQueue.Count > 0)
            {
                text = _textQueue.Dequeue();
                
                //设置文本框大小
                _txtHide.text = text;
                _rect.sizeDelta = new Vector2(_txtHide.preferredWidth + offset, _rect.sizeDelta.y);
                //逐字显示
                _txtDialogue.text = "";
                for (int i = 0; i < text.Length; i++)
                {
                    _txtDialogue.text += text[i];
                    if (_interval > 0)
                        yield return new WaitForSecondsRealtime(_interval);
                    else
                        yield return null;
                }
                
                //显示完再隔一个时间段结束
                if (_sentenceInterval > 0)
                    yield return new WaitForSecondsRealtime(_sentenceInterval);
                else
                    yield return null;
                
                _perSentenceCallbacks[index++]?.Invoke(text);
            }

            _completeCallback?.Invoke(this);
            _isShowing = false;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            _txtDialogue ??= transform.Find("txt_dialogue").GetComponent<Text>();
            _rect ??= GetComponent<RectTransform>();
            EditorApplication.delayCall = () =>
            {
                _txtDialogue.text = text;
                //设置文本框大小
                _rect.sizeDelta = new Vector2(_txtDialogue.preferredWidth + offset, _rect.sizeDelta.y);
            };
        }
    }
}