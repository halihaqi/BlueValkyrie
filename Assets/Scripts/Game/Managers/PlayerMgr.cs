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

        private GameObject _playerObj;
        private GameObject _secretaryObj;

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
                _secretaryInfo = BinaryDataMgr.Instance.GetInfo<RoleInfoContainer, int, RoleInfo>(_nowPlayer.id);
            }
        }

        public RoleInfo NowSecretaryInfo => _secretaryInfo;

        public void SetPlayerPrefab(Camera followCam)
        {
            //加载主角
            if (_playerObj == null)
            {
                ResMgr.Instance.LoadAsync<GameObject>(GameConst.GAME_SCENE, PLAYER_RES_PATH, obj =>
                {
                    _playerObj = obj;
                    _playerObj.GetComponent<PlayerController>().followCamera = followCam;
                });
            }

            //加载秘书
            if(_secretaryInfo == null) return;
            ResMgr.Instance.LoadAsync<GameObject>(GameConst.GAME_SCENE, ResPath.GetStudentObjPath(_secretaryInfo),
                obj =>
                {
                    _secretaryObj = obj;
                });
        }

        public void DestroyPlayerPrefab()
        {
            if(_playerObj == null) return;
            Object.Destroy(_playerObj);
            Object.Destroy(_secretaryObj);
            _playerObj = null;
            _secretaryObj = null;
        }
    }
}