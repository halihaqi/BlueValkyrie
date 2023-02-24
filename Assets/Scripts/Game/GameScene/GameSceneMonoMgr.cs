using UnityEngine;

namespace Game.GameScene
{
    public class GameSceneMonoMgr : SingletonMono<GameSceneMonoMgr>
    {
        [SerializeField]
        private Transform _playerBornPos;

        public Transform PlayerBornPos => _playerBornPos;
    }
}