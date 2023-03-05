using System;
using Game.Managers;
using Game.UI.Controls.Dialogue;
using Game.Utils;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Base
{
    public class DialoguePop : PopBase
    {
        private Image _imgHead;
        private UI_dialogue_frame _dialogueFrame;

        private int _talkerId;
        private Dialogue _dialogue;
        private bool _isAuto;
        private bool _interactable;
        private Action _dialogueCompleteCallback;
        
        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);
            _imgHead = GetControl<Image>("img_head");
            _dialogueFrame = GetControl<UI_dialogue_frame>("dialogue_frame");
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
            _dialogueCompleteCallback = null;
            _dialogueCompleteCallback += HideMe;
            if (userData is DialogueParam p)
            {
                _talkerId = p.roleId;
                _dialogue = p.dialogue;
                _isAuto = p.isAuto;
                _interactable = p.interactable;
                _dialogueCompleteCallback += p.dialogueCompleteCallback;
            }
            else
                throw new Exception("Need correct dialogue.");

            ResMgr.Instance.LoadAsync<Sprite>(GameConst.RES_GROUP_UI, ResPath.GetStudentIcon(_talkerId), img =>
            {
                _imgHead.sprite = img;
            });
            _dialogueFrame.SetData(_dialogue, _interactable, _isAuto);
            
            _dialogueFrame.AddCompleteEvent(_dialogueCompleteCallback);
        }

        protected internal override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            _dialogueFrame.RemoveCompleteEvent(_dialogueCompleteCallback);
            _dialogueCompleteCallback = null;
        }
    }

    public class DialogueParam
    {
        public int roleId;
        public Dialogue dialogue;
        public bool isAuto;
        public bool interactable;
        public Action dialogueCompleteCallback;

        public DialogueParam(int roleId, Dialogue dialogue, bool isAuto, bool interactable,
            Action dialogueCompleteCallback) =>
            (this.roleId, this.dialogue, this.isAuto, this.interactable,
                this.dialogueCompleteCallback) =
            (roleId, dialogue, isAuto, interactable, dialogueCompleteCallback);
    }
}