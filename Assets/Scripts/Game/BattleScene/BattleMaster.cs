using System.Collections.Generic;
using Game.Entity;
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
    
    public class BattleMaster : MonoBehaviour
    {
        private RoundEngine _roundEngine;
        private IFsm<BattleMaster> _battleFsm;
        private bool _isCurRoleEnemy; 
        private int _curRoleIndex = 0;
        private BattleOverType _overType = BattleOverType.NotOver;

        public BattleStudentEntity[] studentEntitys;
        public BattleEnemyEntity[] enemyEntitys;
        public FlagEntity[] flagEntitys;
        public Transform[] shelterPos;
        public OverMapEntity overMapEntity;

        public Camera mapCam;
        public Camera cam;
        public ThirdPersonCam followCam;

        public BattlePanel BattlePanel { get; set; }
        public BattleRoundPanel BattleRoundPanel { get; set; }

        public BattleRoleEntity CurRole =>
            _isCurRoleEnemy ? enemyEntitys[_curRoleIndex] : studentEntitys[_curRoleIndex] as BattleRoleEntity;

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
            studentEntitys = null;
            enemyEntitys = null;
        }

        public void SwitchStudent(BattleStudentEntity student)
        {
            for (int i = 0; i < studentEntitys.Length; i++)
            {
                if (studentEntitys[i] == student)
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
            _curRoleIndex = _curRoleIndex + 1 > enemyEntitys.Length - 1 ? 0 : _curRoleIndex + 1;
        }

        public void SwitchEnemy(BattleEnemyEntity enemy)
        {
            for (int i = 0; i < enemyEntitys.Length; i++)
            {
                if (enemyEntitys[i] == enemy)
                {
                    _isCurRoleEnemy = true;
                    _curRoleIndex = i;
                    EventMgr.Instance.TriggerEvent(ClientEvent.BATTLE_ROLE_CHANGE, enemy);
                    break;
                }
            }
        }

        public BattleStudentEntity GetStudentEntity(int index)
            => studentEntitys[index];
        
        public BattleEnemyEntity GetEnemyEntity(int index)
            => enemyEntitys[index];

        public BattleEnemyEntity GetEnemyEntity(GameObject obj)
        {
            for (int i = 0; i < enemyEntitys.Length; i++)
            {
                if (enemyEntitys[i].gameObject == obj)
                    return enemyEntitys[i];
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
                Dictionary<int, Vector2> dic = new Dictionary<int, Vector2>(enemyEntitys.Length);
                for (int i = 0; i < enemyEntitys.Length; i++)
                {
                    var viewport = mapCam.WorldToViewportPoint(enemyEntitys[i].transform.position);//[0,1]的屏幕映射
                    //映射
                    var targetPos = new Vector2(mapLeftBottom.x + viewport.x * sizeDelta.x,
                        mapLeftBottom.y + viewport.y * sizeDelta.y);
                    dic.Add(i, targetPos);
                }
                return dic;
            }
            else
            {
                Dictionary<int, Vector2> dic = new Dictionary<int, Vector2>(studentEntitys.Length);
                for (int i = 0; i < studentEntitys.Length; i++)
                {
                    var viewport = mapCam.WorldToViewportPoint(studentEntitys[i].transform.position);//[0,1]的屏幕映射
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
            
            Dictionary<int, Vector2> dic = new Dictionary<int, Vector2>(flagEntitys.Length);
            for (int i = 0; i < flagEntitys.Length; i++)
            {
                var viewport = mapCam.WorldToViewportPoint(flagEntitys[i].transform.position);//[0,1]的屏幕映射
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
                Dictionary<int, float> dic = new Dictionary<int, float>(enemyEntitys.Length);
                for (int i = 0; i < enemyEntitys.Length; i++)
                    dic.Add(i, enemyEntitys[i].transform.localEulerAngles.y);
                return dic;
            }
            else
            {
                Dictionary<int, float> dic = new Dictionary<int, float>(studentEntitys.Length);
                for (int i = 0; i < studentEntitys.Length; i++)
                    dic.Add(i, studentEntitys[i].transform.localEulerAngles.y);
                return dic;
            }
        }

        #endregion
        

        public void Atk(bool isCrit, bool isEnemy, int atkerIndex, int deferIndex)
        {
            if (isEnemy)
            {
                var atker = enemyEntitys[atkerIndex];
                var defer = studentEntitys[deferIndex];
                int atk = atker.EnemyInfo.atk * (isCrit ? 3 : 1);
                int def = defer.Student.Def;
                int hit = Mathf.Max(atk - def, 0);
                defer.SubHp(hit);
                if (defer.IsDead)
                {
                    DelayUtils.Instance.Delay(3,1, obj =>
                    {
                        KillRole(true, deferIndex);
                    });
                }
            }
            else
            {
                var atker = studentEntitys[atkerIndex];
                var defer = enemyEntitys[deferIndex];
                int atk = atker.Student.Atk * (isCrit ? 2 : 1);
                int def = defer.EnemyInfo.def;
                int hit = Mathf.Max(atk - def, 0);
                defer.SubHp(hit);
                if (defer.IsDead)
                {
                    DelayUtils.Instance.Delay(3,1, obj =>
                    {
                        KillRole(false, deferIndex);
                    });
                }
            }
        }

        public void KillRole(bool isEnemy, int roleIndex)
        {
            if (isEnemy)
            {
                enemyEntitys[roleIndex].gameObject.SetActive(false);
            }
            else
            {
                studentEntitys[roleIndex].gameObject.SetActive(false);
            }
        }

        public bool IsRoleAroundFlag(BattleRoleEntity role, out FlagEntity flag)
        {
            //先找到最近的旗帜
            flag = flagEntitys[0];
            float nearestDis = float.MaxValue;
            
            foreach (var f in flagEntitys)
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
            for (int i = 0; i < flagEntitys.Length; i++)
            {
                if (flagEntitys[i] == flag)
                {
                    flag.RiseFlag(type);
                    isRise = true;
                    break;
                }
            }

            bool hasStudentFlag = false;
            bool hasEnemyFlag = false;
            for (int i = 0; i < flagEntitys.Length; i++)
            {
                if (flagEntitys[i].FlagType == FlagType.Student)
                    hasStudentFlag = true;
                else if (flagEntitys[i].FlagType == FlagType.Enemy)
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