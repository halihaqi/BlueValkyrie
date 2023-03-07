using System;
using Game.Global;
using Game.Managers;
using Hali_Framework;
using UnityEngine;

namespace Game.GameScene
{
    public class GameSceneMonoMgr : SingletonMono<GameSceneMonoMgr>
    {
        [SerializeField]
        private Transform bornTrans;
        public Vector3 playerBornPos;
        public Quaternion playerBornRotation;

        private void OnValidate()
        {
            if(Application.isPlaying || bornTrans == null) return;
            playerBornPos = bornTrans.position;
            playerBornRotation = bornTrans.rotation;
        }

        private void Start()
        {
            //TestBag();
            TestBattle();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            EventMgr.Instance.RemoveListener<KeyCode>(ClientEvent.GET_KEY_DOWN, OnEnterBattle);
        }

        private void TestBattle()
        {
            EventMgr.Instance.AddListener<KeyCode>(ClientEvent.GET_KEY_DOWN, OnEnterBattle);
        }

        private void OnEnterBattle(KeyCode key)
        {
            if(key == KeyCode.P)
                ProcedureMgr.Instance.ChangeState<BattleProcedure>();
        }
    }
}