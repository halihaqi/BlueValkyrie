using Game.Managers;
using Game.UI.Game;
using Hali_Framework;
using UnityEngine;

namespace Game.GameScene
{
    public class PlayerController : ThirdPersonController
    {
        protected override void Start()
        {
            base.Start();
            //设置自己和秘书初始位置
            transform.position = GameSceneMonoMgr.Instance.playerBornPos;
            transform.rotation = GameSceneMonoMgr.Instance.playerBornRotation;
            EventMgr.Instance.AddListener<KeyCode>(ClientEvent.GET_KEY_DOWN, OnOpenBag);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            EventMgr.Instance.RemoveListener<KeyCode>(ClientEvent.GET_KEY_DOWN, OnOpenBag);
        }

        private void OnOpenBag(KeyCode key)
        {
            if (KeyCode.Tab == key)
            {
                if(!UIMgr.Instance.HasPanel<BagPanel>())
                    UIMgr.Instance.ShowPanel<BagPanel>(userData:0);
            }
        }
    }
}