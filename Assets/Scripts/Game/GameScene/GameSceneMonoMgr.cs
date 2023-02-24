using System;
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
    }
}