﻿using Game.Managers;
using Game.UI.Game;
using Hali_Framework;
using UnityEngine;

namespace Game.GameScene
{
    public class PlayerController : ThirdPersonController
    {
        private int _bagId = -1;
        
        protected override void Start()
        {
            base.Start();
            //设置自己和秘书初始位置
            transform.position = GameSceneMonoMgr.Instance.playerBornPos;
            transform.rotation = GameSceneMonoMgr.Instance.playerBornRotation;
        }

        protected override void Update()
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.B))
            {
                if (UIMgr.Instance.HasPanel(_bagId))
                    UIMgr.Instance.HidePanel(_bagId);
                else
                    _bagId = UIMgr.Instance.ShowPanel<BagPanel>(userData:0);
            }
        }
    }
}