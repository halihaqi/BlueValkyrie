using Game.Managers;
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
        }
    }
}