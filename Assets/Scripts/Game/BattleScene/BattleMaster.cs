using System.Collections.Generic;
using Game.BattleScene.BattleRole;
using Game.Entity;
using Game.Model;
using Game.UI.Battle;
using Hali_Framework;
using UnityEngine;

namespace Game.BattleScene
{
    public enum BattleOverType
    {
        NotOver,
        StudentWin,
        EnemyWin,
        Draw,
        ShutDown,
    }

    public class CampInfo
    {
        public RoleType campType;
        public List<FlagEntity> flags;
        public IBattleRole commander;//指挥官
        public List<IBattleRole> soldiers;
    }

    public class BattleMaster : MonoBehaviour
    {
        private RoundEngine _roundEngine;
        private IFsm<BattleMaster> _battleFsm;
        private BattleOverType _overType = BattleOverType.NotOver;
        

        //战斗阵营
        private Dictionary<RoleType, CampInfo> _camps;
        private List<RoleType> _campTypes;//用于记录参与战斗的阵营类型
        private IBattleRole _curRole;
        private Transform[] shelterPos;

        public Camera mapCam;
        public ThirdPersonCam followCam;

        public BattlePanel BattlePanel { get; set; }
        public BattleRoundPanel BattleRoundPanel { get; set; }

        public IBattleRole CurRole => _curRole;

        public BattleOverType OverType => _overType;

        public List<RoleType> CampTypes => _campTypes;
        
        //回合属性
        public CampRoundInfo CurCamp => _roundEngine.CurCamp;
        public int CurRound => _roundEngine.CurRound;
        public int MaxRound => _roundEngine.MaxRound;

        private void Awake()
        {
            _camps = new Dictionary<RoleType, CampInfo>();
            _campTypes = new List<RoleType>();
            //初始化回合
            _roundEngine = new RoundEngine();
            var episodeInfo = ProcedureMgr.Instance.GetData<EpisodeInfo>(BattleConst.MAP_KEY);
            _roundEngine.Init(episodeInfo.round, BattleConst.ROUND_AP);
            
            //创建战斗状态机
            _battleFsm = FsmMgr.Instance.CreateFsm(BattleConst.BATTLE_FSM, this, 
                new BattleInitState(), new BattleStartState(), 
                new BattleChessState(), new BattleRunState(), new BattleOverState());
        }

        private void Start()
        {
            _battleFsm.Start<BattleInitState>();
            _overType = BattleOverType.NotOver;
        }

        private void OnDestroy()
        {
            FsmMgr.Instance.DestroyFsm(BattleConst.BATTLE_FSM);

            _battleFsm = null;
            followCam = null;
            mapCam = null;
        }

        #region 处理士兵

        /// <summary>
        /// 添加作战阵营
        /// </summary>
        /// <param name="camp"></param>
        public void JoinCamp(CampInfo camp)
        {
            if (!_camps.ContainsKey(camp.campType))
            {
                _camps.Add(camp.campType, camp);
                _campTypes.Add(camp.campType);
                _roundEngine.AddCamp(camp.campType);
            }
            else
                Debug.Log($"Cannot add same camp: {camp.campType}.");
        }

        public void ChangeRole(IBattleRole role)
        {
            var type = role.RoleType;
            if (!_camps.ContainsKey(type)) return;
            
            foreach (var soldier in _camps[type].soldiers)
            {
                //切换
                if (soldier == role)
                {
                    _curRole = role;
                    //todo 切换可能需要的逻辑
                    SetFollowTarget(role);
                    return;
                }
            }
        }

        public IBattleRole GetRole(RoleType type, int roleId)
        {
            if (_camps.TryGetValue(type, out var camp))
            {
                var role = camp.soldiers.Find(o => o.RoleId == roleId);
                return role;
            }
            return null;
        }

        public List<IBattleRole> GetRoles(RoleType type)
        {
            if (_camps.TryGetValue(type, out var camp))
                return camp.soldiers;
            return null;
        }
        
        public List<FlagEntity> GetFlags(RoleType type)
        {
            if (_camps.TryGetValue(type, out var camp))
                return camp.flags;
            return null;
        }

        public void Atk(IBattleRole atker, IBattleRole defer)
        {
            //先计算伤害
            int hit = BattleComputeHelper.Atk(atker.RoleState, defer.RoleState);
            defer.SubHp(hit);
            if (defer.IsDead)
            {
                Debug.Log($"{defer.Go.name} 死亡.");
                //todo 死亡后处理
            }
        }

        #endregion

        #region 处理棋子

        /// <summary>
        /// 获得士兵在地图中的位置
        /// </summary>
        /// <param name="type"></param>
        /// <param name="mapRect"></param>
        /// <returns></returns>
        public Dictionary<IBattleRole, Vector2> GetRoleChessStation(RoleType type, RectTransform mapRect)
        {
            if (!_camps.TryGetValue(type, out var camp)) return null;
            Dictionary<IBattleRole, Vector2> res = new Dictionary<IBattleRole, Vector2>(camp.soldiers.Count);
            
            Vector2 sizeDelta = mapRect.sizeDelta;
            var mapLb = mapRect.anchoredPosition - sizeDelta / 2;//左下
            for (int i = 0; i < camp.soldiers.Count; i++)
            {
                var viewport = mapCam.WorldToViewportPoint(camp.soldiers[i].Go.transform.position);//[0,1]的屏幕映射
                //映射
                var targetPos = new Vector2(mapLb.x + viewport.x * sizeDelta.x,
                    mapLb.y + viewport.y * sizeDelta.y);
                res.Add(camp.soldiers[i], targetPos);
            }

            return res;
        }

        /// <summary>
        /// 获得士兵在地图中的旋转
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Dictionary<IBattleRole, float> GetRoleChessRotation(RoleType type)
        {
            if (!_camps.TryGetValue(type, out var camp)) return null;
            Dictionary<IBattleRole, float> res = new Dictionary<IBattleRole, float>(camp.soldiers.Count);
            for (int i = 0; i < camp.soldiers.Count; i++)
                res.Add(camp.soldiers[i], camp.soldiers[i].Go.transform.localEulerAngles.y);
            return res;
        }

        /// <summary>
        /// 获取旗帜在地图中的位置
        /// </summary>
        /// <param name="type"></param>
        /// <param name="mapRect"></param>
        /// <returns></returns>
        public Dictionary<FlagEntity, Vector2> GetFlagChessStation(RoleType type, RectTransform mapRect)
        {
            if (!_camps.TryGetValue(type, out var camp)) return null;
            Dictionary<FlagEntity, Vector2> res = new Dictionary<FlagEntity, Vector2>(camp.flags.Count);
            
            Vector2 sizeDelta = mapRect.sizeDelta;
            var mapLb = mapRect.anchoredPosition - sizeDelta / 2;//左下
            for (int i = 0; i < camp.flags.Count; i++)
            {
                var viewport = mapCam.WorldToViewportPoint(camp.flags[i].transform.position);//[0,1]的屏幕映射
                //映射
                var targetPos = new Vector2(mapLb.x + viewport.x * sizeDelta.x,
                    mapLb.y + viewport.y * sizeDelta.y);
                res.Add(camp.flags[i], targetPos);
            }

            return res;
        }

        #endregion

        #region 处理相机

        private void SetFollowTarget(IBattleRole role)
        {
            if (role == null || !_camps.ContainsKey(role.RoleType)) return;
            if(!_camps.ContainsKey(role.RoleType)) return;
            followCam.followTarget = role.FollowTarget;
        }

        #endregion

        #region 处理回合

        public void WarStart(RoleType type)
        {
            _roundEngine.Start(type);
        }

        public void WarRun()
        {
            _roundEngine.Run();
            EventMgr.Instance.TriggerEvent(ClientEvent.BATTLE_ROUND_RUN);
        }

        public void WarCampOver()
        {
            _roundEngine.CampOver();
        }

        #endregion
        
        
        // public bool IsRoleAroundFlag(BattleRoleEntity role, out FlagEntity flag)
        // {
        //     //先找到最近的旗帜
        //     flag = flags[0];
        //     float nearestDis = float.MaxValue;
        //     
        //     foreach (var f in flags)
        //     {
        //         var dis = Vector3.Distance(role.transform.position, f.transform.position);
        //         if (dis <= nearestDis)
        //         {
        //             nearestDis = dis;
        //             flag = f;
        //         }
        //     }
        //     
        //     //判断是否在旗帜周围
        //     return flag.IsRoleAround(role);
        // }
        //
        // public void RiseFlag(FlagEntity flag, FlagType type)
        // {
        //     bool isRise = false;
        //     for (int i = 0; i < flags.Length; i++)
        //     {
        //         if (flags[i] == flag)
        //         {
        //             flag.RiseFlag(type);
        //             isRise = true;
        //             break;
        //         }
        //     }
        //
        //     bool hasStudentFlag = false;
        //     bool hasEnemyFlag = false;
        //     for (int i = 0; i < flags.Length; i++)
        //     {
        //         if (flags[i].FlagType == FlagType.Student)
        //             hasStudentFlag = true;
        //         else if (flags[i].FlagType == FlagType.Enemy)
        //             hasEnemyFlag = true;
        //     }
        //
        //     //学生赢
        //     if (!hasEnemyFlag)
        //     {
        //         _overType = BattleOverType.StudentWin;
        //         _battleFsm.ChangeState<BattleOverState>();
        //     }
        //     //敌人赢
        //     else if (!hasStudentFlag)
        //     {
        //         _overType = BattleOverType.EnemyWin;
        //         _battleFsm.ChangeState<BattleOverState>();
        //     }
        // }
    }
}