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
        
        private IBattleRole _curRole;
        private int _curRoleIndex = 0;

        //战斗阵营
        private Dictionary<RoleType, CampInfo> _camps;
        private IBattleRole[] students;
        private IBattleRole[] enemies;
        private FlagEntity[] flags;
        private Transform[] shelterPos;
        private OverMapEntity overMapEntity;

        public Camera mapCam;
        public Camera cam;
        public ThirdPersonCam followCam;

        public BattlePanel BattlePanel { get; set; }
        public BattleRoundPanel BattleRoundPanel { get; set; }

        public IBattleRole CurRole => _curRole;

        public RoundEngine RoundEngine => _roundEngine;

        public BattleOverType OverType => _overType;
        
        private void Awake()
        {
            //初始化回合
            _roundEngine = new RoundEngine();
            var episodeInfo = ProcedureMgr.Instance.GetData<EpisodeInfo>(BattleConst.MAP_KEY);
            _roundEngine.Init(false, episodeInfo.round, BattleConst.ROUND_AP);
            
            //创建战斗状态机
            _battleFsm = FsmMgr.Instance.CreateFsm(BattleConst.BATTLE_FSM, this, 
                new BattleInitState(), new BattleStartState(), 
                new BattleArrayState(), new BattleRunState(), new BattleOverState());
        }

        private void Start()
        {
            _battleFsm.Start<BattleInitState>();
            _overType = BattleOverType.NotOver;
            overMapEntity.gameObject.SetActive(false);
            EventMgr.Instance.AddListener(ClientEvent.BATTLE_STEP_OVER, OnBattleStepOver);
            EventMgr.Instance.AddListener(ClientEvent.BATTLE_ROUND_RUN, OnBattleRoundRun);
        }

        private void OnDestroy()
        {
            FsmMgr.Instance.DestroyFsm(BattleConst.BATTLE_FSM);
            EventMgr.Instance.RemoveListener(ClientEvent.BATTLE_STEP_OVER, OnBattleStepOver);
            EventMgr.Instance.RemoveListener(ClientEvent.BATTLE_ROUND_RUN, OnBattleRoundRun);

            _battleFsm = null;
            cam = null;
            followCam = null;
            mapCam = null;
            students = null;
            enemies = null;
        }

        /// <summary>
        /// 添加作战阵营
        /// </summary>
        /// <param name="camp"></param>
        public bool AddCamp(CampInfo camp)
        {
            if (!_camps.ContainsKey(camp.campType))
            {
                _camps.Add(camp.campType, camp);
                return true;
            }
            return false;
        }

        public void SwitchStudent(BattleStudentEntity student)
        {
            for (int i = 0; i < students.Length; i++)
            {
                if (students[i] == student)
                {
                    _isCurRoleEnemy = false;
                    _curRoleIndex = i;
                    EventMgr.Instance.TriggerEvent(ClientEvent.BATTLE_ROLE_CHANGE, student);
                    break;
                }
            }
        }

        public void AutoEnemyIndex()
        {
            _isCurRoleEnemy = true;
            _curRoleIndex = _curRoleIndex + 1 > enemies.Length - 1 ? 0 : _curRoleIndex + 1;
        }

        public void SwitchEnemy(BattleEnemyEntity enemy)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i] == enemy)
                {
                    _isCurRoleEnemy = true;
                    _curRoleIndex = i;
                    EventMgr.Instance.TriggerEvent(ClientEvent.BATTLE_ROLE_CHANGE, enemy);
                    break;
                }
            }
        }

        public IBattleRole GetStudent(int index)
            => students[index];
        
        public IBattleRole GetEnemy(int index)
            => enemies[index];

        public IBattleRole GetEnemy(GameObject obj)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i].Go == obj)
                    return enemies[i];
            }
            return null;
        }

        #region 棋子地图位置

        public Dictionary<int, Vector2> CalcRolesMapPos(bool isEnemy, RectTransform mapRect)
        {
            Vector2 sizeDelta = mapRect.sizeDelta;
            var mapLeftBottom = mapRect.anchoredPosition - sizeDelta / 2;//左下

            if (isEnemy)
            {
                Dictionary<int, Vector2> dic = new Dictionary<int, Vector2>(enemies.Length);
                for (int i = 0; i < enemies.Length; i++)
                {
                    var viewport = mapCam.WorldToViewportPoint(enemies[i].Go.transform.position);//[0,1]的屏幕映射
                    //映射
                    var targetPos = new Vector2(mapLeftBottom.x + viewport.x * sizeDelta.x,
                        mapLeftBottom.y + viewport.y * sizeDelta.y);
                    dic.Add(i, targetPos);
                }
                return dic;
            }
            else
            {
                Dictionary<int, Vector2> dic = new Dictionary<int, Vector2>(students.Length);
                for (int i = 0; i < students.Length; i++)
                {
                    var viewport = mapCam.WorldToViewportPoint(students[i].transform.position);//[0,1]的屏幕映射
                    //映射
                    var targetPos = new Vector2(mapLeftBottom.x + viewport.x * sizeDelta.x,
                        mapLeftBottom.y + viewport.y * sizeDelta.y);
                    dic.Add(i, targetPos);
                }
                return dic;
            }
        }

        public Dictionary<int, Vector2> CalcFlagMapPos(RectTransform mapRect)
        {
            Vector2 sizeDelta = mapRect.sizeDelta;
            var mapLeftBottom = mapRect.anchoredPosition - sizeDelta / 2;//左下
            
            Dictionary<int, Vector2> dic = new Dictionary<int, Vector2>(flags.Length);
            for (int i = 0; i < flags.Length; i++)
            {
                var viewport = mapCam.WorldToViewportPoint(flags[i].transform.position);//[0,1]的屏幕映射
                //映射
                var targetPos = new Vector2(mapLeftBottom.x + viewport.x * sizeDelta.x,
                    mapLeftBottom.y + viewport.y * sizeDelta.y);
                dic.Add(i, targetPos);
            }

            return dic;
        }
        
        public Dictionary<int, float> CalcRolesMapRotation(bool isEnemy)
        {
            if (isEnemy)
            {
                Dictionary<int, float> dic = new Dictionary<int, float>(enemies.Length);
                for (int i = 0; i < enemies.Length; i++)
                    dic.Add(i, enemies[i].transform.localEulerAngles.y);
                return dic;
            }
            else
            {
                Dictionary<int, float> dic = new Dictionary<int, float>(students.Length);
                for (int i = 0; i < students.Length; i++)
                    dic.Add(i, students[i].transform.localEulerAngles.y);
                return dic;
            }
        }

        #endregion
        

        public void Atk(bool isCrit, bool isEnemy, int atkerIndex, int deferIndex)
        {
            if (isEnemy)
            {
                var atker = enemies[atkerIndex];
                var defer = students[deferIndex];
                int atk = atker.EnemyInfo.atk * (isCrit ? 3 : 1);
                int def = defer.Student.Def;
                int hit = Mathf.Max(atk - def, 0);
                defer.SubHp(hit);
                Debug.Log($"{atker.name} 对 {defer.name} 造成 {hit} 点伤害.");
                if (defer.IsDead)
                {
                    Debug.Log($"{defer.name} 死亡.");
                    DelayUtils.Instance.Delay(3,1, obj =>
                    {
                        KillRole(true, deferIndex);
                    });
                }
            }
            else
            {
                var atker = students[atkerIndex];
                var defer = enemies[deferIndex];
                int atk = atker.Student.Atk * (isCrit ? 2 : 1);
                int def = defer.EnemyInfo.def;
                int hit = Mathf.Max(atk - def, 0);
                defer.SubHp(hit);
                Debug.Log($"{atker.name} 对 {defer.name} 造成 {hit} 点伤害.");
                if (defer.IsDead)
                {
                    DelayUtils.Instance.Delay(3,1, obj =>
                    {
                        Debug.Log($"{defer.name} 死亡.");
                        KillRole(false, deferIndex);
                    });
                }
            }
        }

        public void KillRole(bool isEnemy, int roleIndex)
        {
            if (isEnemy)
            {
                enemies[roleIndex].gameObject.SetActive(false);
            }
            else
            {
                students[roleIndex].gameObject.SetActive(false);
            }
        }

        public bool IsRoleAroundFlag(BattleRoleEntity role, out FlagEntity flag)
        {
            //先找到最近的旗帜
            flag = flags[0];
            float nearestDis = float.MaxValue;
            
            foreach (var f in flags)
            {
                var dis = Vector3.Distance(role.transform.position, f.transform.position);
                if (dis <= nearestDis)
                {
                    nearestDis = dis;
                    flag = f;
                }
            }
            
            //判断是否在旗帜周围
            return flag.IsRoleAround(role);
        }

        public void RiseFlag(FlagEntity flag, FlagType type)
        {
            bool isRise = false;
            for (int i = 0; i < flags.Length; i++)
            {
                if (flags[i] == flag)
                {
                    flag.RiseFlag(type);
                    isRise = true;
                    break;
                }
            }

            bool hasStudentFlag = false;
            bool hasEnemyFlag = false;
            for (int i = 0; i < flags.Length; i++)
            {
                if (flags[i].FlagType == FlagType.Student)
                    hasStudentFlag = true;
                else if (flags[i].FlagType == FlagType.Enemy)
                    hasEnemyFlag = true;
            }

            //学生赢
            if (!hasEnemyFlag)
            {
                _overType = BattleOverType.StudentWin;
                _battleFsm.ChangeState<BattleOverState>();
            }
            //敌人赢
            else if (!hasStudentFlag)
            {
                _overType = BattleOverType.EnemyWin;
                _battleFsm.ChangeState<BattleOverState>();
            }
        }

        private void OnBattleStepOver()
        {
            _roundEngine.Run();
        }

        private void OnBattleRoundRun()
        {
            if(_battleFsm.CurrentState.GetType() != typeof(BattleArrayState))
                _battleFsm.ChangeState<BattleArrayState>();
        }
    }
}