using UnityEngine;

namespace Game.GameScene
{
    public class GameSceneMonoMgr : SingletonMono<GameSceneMonoMgr>
    {
        [SerializeField]
        private Transform bornTrans;
        public Vector3 playerBornPos;
        public Quaternion playerBornRotation;

        protected override void Awake()
        {
            base.Awake();
            if(Application.isPlaying || bornTrans == null) return;
            playerBornPos = bornTrans.position;
            playerBornRotation = bornTrans.rotation;
        }
    }
}