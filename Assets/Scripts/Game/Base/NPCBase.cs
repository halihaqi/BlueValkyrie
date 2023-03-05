using System;
using Game.Managers;
using Game.UI.Base;
using Hali_Framework;
using UnityEngine;

namespace Game.Base
{
    public class NPCBase : RoleBase
    {
        [Header("Dialogue")]
        [SerializeField]
        private int roleId = 1001;
        [SerializeField]
        private TextAsset asset;
        [SerializeField]
        private float interval = 2f;
        private Dialogue _dialogue;
        
        private int _panelId = -1;
        private static readonly int Reaction = Animator.StringToHash("reaction");

        protected override void Awake()
        {
            base.Awake();
            SetRoleInfo(roleId);
            _dialogue = new Dialogue(asset, interval);
            for (int i = 0; i < _dialogue.callbacks.Count; i++)
                _dialogue.AddSentenceCallback(i, OnPerSentence);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(GameConst.PLAYER_TAG)) return;
            if(UIMgr.Instance.HasPanel<DialoguePop>()) return;
            
            anim.SetTrigger(Reaction);
            _panelId = UIMgr.Instance.ShowPanel<DialoguePop>(GameConst.UIGROUP_POP,
                userData: new DialogueParam(roleId, _dialogue, true, true, OnDialogueOver));
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(GameConst.PLAYER_TAG)) return;
            
            UIMgr.Instance.HidePanel(_panelId);
        }

        protected virtual void OnPerSentence(string str)
        {
        }

        protected virtual void OnDialogueOver()
        {
        }
    }
}