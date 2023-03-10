using Game.GameScene;
using Game.Model;
using Game.Utils;
using Hali_Framework;
using UnityEngine;

namespace Game.Managers
{
    public class PlayerMgr : Singleton<PlayerMgr>
    {
        private PlayerInfo _nowPlayer;
        private RoleInfo _secretaryInfo;

        private PlayerController _playerEntity;
        private SecretaryEntity _secretaryEntity;

        private const string PLAYER_RES_PATH = "Prefabs/Player/Player"; 
        public const string PLAYER_DATA_KEY = "Player";

        public PlayerInfo NowPlayer
        {
            get => _nowPlayer;
            set
            {
                _nowPlayer = value;
                if (_nowPlayer == null)
                {
                    _secretaryInfo = null;
                    return;
                }
                _secretaryInfo = BinaryDataMgr.Instance.GetInfo<RoleInfoContainer, int, RoleInfo>(_nowPlayer.secretaryId);
                BagMgr = new BagMgr(_nowPlayer);
            }
        }
        
        public BagMgr BagMgr { get; private set; }

        public RoleInfo NowSecretaryInfo => _secretaryInfo;

        public bool HasPlayer => _playerEntity != null;

        public bool HasSecretary => _secretaryEntity != null;

        public void SetPlayerPrefab(Camera followCam)
        {
            //加载主角
            if (_playerEntity == null)
            {
                ResMgr.Instance.LoadAsync<GameObject>(GameConst.GAME_SCENE, PLAYER_RES_PATH, obj =>
                {
                    _playerEntity = obj.GetComponent<PlayerController>();
                    _playerEntity.followCamera = followCam;
                });
            }

            //加载秘书
            if(_secretaryInfo == null) return;
            ResMgr.Instance.LoadAsync<GameObject>(GameConst.GAME_SCENE, ResPath.GetStudentObj(_secretaryInfo),
                obj =>
                {
                    _secretaryEntity = obj.AddComponent<SecretaryEntity>();
                    var playerTrans = _playerEntity.transform;
                    _secretaryEntity.FollowTarget = playerTrans;
                    _secretaryEntity.SetFollowDistance(2);
                });
        }

        public void DestroyPlayerPrefab()
        {
            if(_playerEntity == null) return;
            Object.Destroy(_playerEntity.gameObject);
            Object.Destroy(_secretaryEntity.gameObject);
            _playerEntity = null;
            _secretaryEntity = null;
        }
    }
}