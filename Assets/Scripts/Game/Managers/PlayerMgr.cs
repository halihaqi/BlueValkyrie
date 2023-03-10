using System.Collections.Generic;
using System.Linq;
using Game.GameScene;
using Game.Model;
using Game.Utils;
using Hali_Framework;
using UnityEngine;

namespace Game.Managers
{
    public class PlayerMgr : Singleton<PlayerMgr>, IModule
    {
        private PlayerItem _curPlayer;
        private RoleInfo _secretaryInfo;

        private PlayerController _playerEntity;
        private SecretaryEntity _secretaryEntity;

        private const string PLAYER_RES_PATH = "Prefabs/Player/Player"; 
        public const string PLAYER_DATA_KEY = "Player";

        public int Priority => 2;
        void IModule.Init()
        {
        }

        void IModule.Update(float elapseSeconds, float realElapseSeconds)
        {
            if(_curPlayer == null) return;
            _curPlayer.time += realElapseSeconds;
        }

        void IModule.Dispose()
        {
            _curPlayer = null;
            _secretaryInfo = null;
            if(_playerEntity != null)
                Object.DestroyImmediate(_playerEntity.gameObject);
            if(_secretaryEntity != null)
                Object.DestroyImmediate(_secretaryEntity.gameObject);
            _playerEntity = null;
            _secretaryEntity = null;
        }

        public PlayerItem CurPlayer
        {
            get => _curPlayer;
            set
            {
                _curPlayer = value;
                if (_curPlayer == null)
                {
                    _secretaryInfo = null;
                    return;
                }
                _secretaryInfo = RoleMgr.Instance.GetRole(_curPlayer.secretaryId);
                BagMaster = new BagMaster(_curPlayer);
                ShopMaster = new BagMaster(_curPlayer.ShopItem);
            }
        }
        
        public BagMaster BagMaster { get; private set; }
        
        public BagMaster ShopMaster { get; private set; }

        public RoleInfo CurSecretaryInfo => _secretaryInfo;

        public bool HasPlayer => _playerEntity != null;

        public bool HasSecretary => _secretaryEntity != null;

        public void RegisterPlayerEntity(Camera followCam)
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
        
        public void UnloadPlayerEntity()
        {
            if(_playerEntity == null) return;
            Object.Destroy(_playerEntity.gameObject);
            Object.Destroy(_secretaryEntity.gameObject);
            _playerEntity = null;
            _secretaryEntity = null;
        }

        public void SaveUser(int userId, PlayerItem item)
        {
            var playerData = BinaryDataMgr.Instance.Load<PlayerData>(GameConst.DATA_PART_PLAYER, "PlayerData");
            if(playerData.dataDic.ContainsKey(userId))
                playerData.dataDic[userId] = item;
            else
                playerData.dataDic.Add(userId, item);
            BinaryDataMgr.Instance.Save(GameConst.DATA_PART_PLAYER, "PlayerData", playerData);
        }

        public PlayerItem LoadUser(int userId)
        {
            var dic = BinaryDataMgr.Instance.Load<PlayerData>(GameConst.DATA_PART_PLAYER, "PlayerData").dataDic;
            return dic.TryGetValue(userId, out var info) ? info : null;
        }
        
        public PlayerItem LoadUser(string name)
        {
            var dic = BinaryDataMgr.Instance.Load<PlayerData>(GameConst.DATA_PART_PLAYER, "PlayerData").dataDic;
            var kv = dic.FirstOrDefault(o => o.Value.name.Equals(name));
            return kv.Value;
        }

        public Dictionary<int, PlayerItem> LoadUserDic()
        {
            return BinaryDataMgr.Instance.Load<PlayerData>(GameConst.DATA_PART_PLAYER, "PlayerData").dataDic;
        }
    }
}