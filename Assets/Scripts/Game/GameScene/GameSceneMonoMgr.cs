using System;
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
            TestBag();
        }

        private void TestBag()
        {
            var bagMgr = PlayerMgr.Instance.BagMaster;
            for (int i = 1; i <= 30; i++)
            {
                bagMgr.AddItem(0, i, 10);
            }
        }
    }
}