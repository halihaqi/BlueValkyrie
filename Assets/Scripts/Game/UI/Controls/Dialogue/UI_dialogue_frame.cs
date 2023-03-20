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
        //逐字显示间隔
        private const float INTERVAL = 0.1f;

        [SerializeField]
        private string text;
        [SerializeField]
        private float offset = 160;
        [SerializeField]
        private float skipCd = 0.3f;

        private RectTransform _rect;
        private RectTransform _arrow;
        private float _arrowOriY;
        private Sequence _arrowTween;
        private Text _txtDialogue;
        //为了逐字显示时文本框固定，用一个隐藏的文本框规定大小
        //再通过改变显示的文本框实现逐字显示
        private Text _txtHide;

        private float _skipCdDelta;
        private float _sentenceInterval = 1;
        private bool _isShowing = false;
        private int _sentenceCount;
        private bool _isAuto;
        private bool _interactable;
        private bool _interactionMark;//是否互动了
        private bool _isLoadingOneSentence;//是否正处于一句话逻辑中
        
        private Queue<string> _textQueue;
        private Dictionary<int, Action<string>> _perSentenceCallbacks;
        private Action _completeCallback;
        
        public bool IsShowing => _isShowing;

        protected internal override void OnInit()
        {
            base.OnInit();
            _txtDialogue = GetControl<Text>("txt_dialogue");
            _txtHide = GetControl<Text>("txt_hide");
            
            _arrow = GetControl<Image>("img_next").rectTransform;
            _rect = this.transform as RectTransform;
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            StopAllCoroutines();
            EventMgr.Instance.RemoveListener<KeyCode>(ClientEvent.GET_KEY_DOWN, OnInteraction);
        }

        private void Update()
        {
            _skipCdDelta -= Time.deltaTime;
        }

        public void SetData(string[] strs, float sentenceInterval, bool interactable, bool isAuto)
        {
            _textQueue = new Queue<string>();
            EventMgr.Instance.AddListener<KeyCode>(ClientEvent.GET_KEY_DOWN, OnInteraction);
            _textQueue.Clear();
            foreach (var str in strs)
                _textQueue.Enqueue(str);
            _isAuto = isAuto;
            _interactable = interactable;
            _sentenceCount = _textQueue.Count;
            _sentenceInterval = sentenceInterval;

            SetArrowActive(interactable);
            StartCoroutine(VerbatimShowSentences());
        }

        public void SetData(Managers.Dialogue dialogue, bool interactable, bool isAuto)
        {
            _perSentenceCallbacks = dialogue.callbacks;
            SetData(dialogue.text, dialogue.interval, interactable, isAuto);
        }

        public void AddCompleteEvent(Action callback)
        {
            _completeCallback -= callback;
            _completeCallback += callback;
        }
        
        public void RemoveCompleteEvent(Action callback)
        {
            _completeCallback -= callback;
        }

        public void Skip()
        {
            if(_skipCdDelta > 0 && !_isLoadingOneSentence) return;
            _skipCdDelta = skipCd;
            
            
            StopAllCoroutines();
            //播放最后一句时跳过
            if (_textQueue.Count <= 0)
            {
                SetArrowActive(false);
                _txtDialogue.text = text;
                _perSentenceCallbacks[_sentenceCount - 1]?.Invoke(text);
                _completeCallback?.Invoke();
                _isShowing = false;
            }
            else
            {
                SetArrowActive(true);
                _txtDialogue.text = text;
                _perSentenceCallbacks[_sentenceCount - _textQueue.Count - 1]?.Invoke(text);
                StartCoroutine(VerbatimShowSentences());
            }
        }
        
        

        private void OnInteraction(KeyCode key)
        {
            if(_interactable && key != KeyCode.E) return;
            _interactionMark = true;
            Skip();
        }

        private void SetArrowActive(bool enable)
        {
            _arrow.gameObject.SetActive(enable);
        }

        private IEnumerator VerbatimShowSentences()
        {
            _isShowing = true;
            while (_textQueue.Count > 0)
            {
                _isLoadingOneSentence = true;
                //最后一句话不用显示Arrow
                if(_textQueue.Count == 1)
                    SetArrowActive(false);
                text = _textQueue.Dequeue();
                
                //设置文本框大小
                _txtHide.text = text;
                _rect.sizeDelta = new Vector2(_txtHide.preferredWidth + offset, _rect.sizeDelta.y);
                //逐字显示
                _txtDialogue.text = "";
                for (int i = 0; i < text.Length; i++)
                {
                    _txtDialogue.text += text[i];
                    yield return new WaitForSeconds(INTERVAL);
                }
                
                //显示完再隔一个时间段结束
                if (_sentenceInterval > 0)
                    yield return new WaitForSecondsRealtime(_sentenceInterval);
                else
                    yield return null;
                
                _perSentenceCallbacks[_sentenceCount - _textQueue.Count - 1]?.Invoke(text);
                _isLoadingOneSentence = false;

                while (!_isAuto)
                {
                    if(!_interactionMark)
                        yield return null;
                    else
                    {
                        _interactionMark = false;
                        yield break;
                    }
                }
            }

            _completeCallback?.Invoke();
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