namespace Game.GameScene
{
    public class PlayerController : ThirdPersonController
    {
        protected override void Start()
        {
            base.Start();
            transform.position = GameSceneMonoMgr.Instance.PlayerBornPos.position;
            transform.rotation = GameSceneMonoMgr.Instance.PlayerBornPos.rotation;
        }
    }
}