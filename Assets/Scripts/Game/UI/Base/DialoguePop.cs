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

        private bool _isAutoHide;
        private int _talkerId;
        private Dialogue _dialogue;
        
        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);
            _imgHead = GetControl<Image>("img_head");
            _dialogueFrame = GetControl<UI_dialogue_frame>("dialogue_frame");
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
            if (userData is DialogueParam p)
            {
                _talkerId = p.roleId;
                _dialogue = p.dialogue;
                _isAutoHide = p.isAutoHide;
            }
            
            ResMgr.Instance.LoadAsync<Sprite>(GameConst.RES_GROUP_UI, ResPath.GetStudentIcon(_talkerId), img =>
            {
                _imgHead.sprite = img;
            });
            _dialogueFrame.SetData(_dialogue);
            if(_isAutoHide)
                _dialogueFrame.AddCompleteEvent(OnDialogueComplete);
        }

        protected internal override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            if(_isAutoHide)
                _dialogueFrame.RemoveCompleteEvent(OnDialogueComplete);
        }

        private void OnDialogueComplete(UI_dialogue_frame control)
        {
            HideMe();
        }
    }

    public class DialogueParam
    {
        public int roleId;
        public Dialogue dialogue;
        public bool isAutoHide;

        public DialogueParam(int roleId, Dialogue dialogue, bool isAutoHide) =>
            (this.roleId, this.dialogue, this.isAutoHide) = (roleId, dialogue, isAutoHide);
    }
}