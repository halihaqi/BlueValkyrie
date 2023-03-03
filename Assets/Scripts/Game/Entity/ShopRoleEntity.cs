using System;
using Game.Base;
using Game.Managers;
using Game.Model;
using Game.UI.Base;
using Game.UI.Game;
using Hali_Framework;
using UnityEngine;

namespace Game.Entity
{
    public class ShopRoleEntity : RoleBase
    {
        [Header("Shop")]
        [SerializeField]
        private int roleId = 1001;
        [SerializeField]
        private TextAsset asset;
        [SerializeField]
        private float interval = 2f;

        private BagMaster _master;
        private Dialogue _dialogue;

        private int _panelId = -1;
        private static readonly int Reaction = Animator.StringToHash("reaction");

        protected override void Awake()
        {
            base.Awake();
            SetRoleInfo(roleId);
            _master = PlayerMgr.Instance.ShopMaster;
            _dialogue = new Dialogue(asset, interval);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(GameConst.PLAYER_TAG)) return;
            if(UIMgr.Instance.HasPanel<DialoguePop>()) return;
            
            anim.SetTrigger(Reaction);
            _panelId = UIMgr.Instance.ShowPanel<DialoguePop>(GameConst.UIGROUP_POP,
                userData: new DialogueParam(roleId, _dialogue, false));
            EventMgr.Instance.AddListener<KeyCode>(ClientEvent.GET_KEY_DOWN, OnEnterShop);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(GameConst.PLAYER_TAG)) return;
            
            UIMgr.Instance.HidePanel(_panelId);
            EventMgr.Instance.RemoveListener<KeyCode>(ClientEvent.GET_KEY_DOWN, OnEnterShop);
        }

        private void OnEnterShop(KeyCode key)
        {
            if (key == KeyCode.E)
                UIMgr.Instance.ShowPanel<ShopPanel>();
        }
    }
}