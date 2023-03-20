using System;
using Game.Entity;
using Game.Managers;
using Game.UI.Battle;
using Game.Utils;
using Hali_Framework;
using UnityEngine;

namespace Game.BattleScene
{
    public class BattleInitState : FsmState<BattleMaster>
    {
        private bool _mapComplete;
        private bool _studentComplete;
        private bool _enemyComplete;

        private EpisodeEntity _episode;
        private EpisodeInfo _episodeInfo;
        
        protected internal override void OnEnter(IFsm<BattleMaster> fsm)
        {
            base.OnEnter(fsm);
            InitPanel(fsm);
            InitMap(fsm);
        }

        protected internal override void OnUpdate(IFsm<BattleMaster> fsm, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(fsm, elapseSeconds, realElapseSeconds);
            if (_mapComplete)
            {
                _mapComplete = false;
                InitStudents(fsm);
                InitEnemy(fsm);
            }
            
            if(!_studentComplete || !_enemyComplete) return;
            ChangeState<BattleStartState>(fsm);
        }

        protected internal override void OnLeave(IFsm<BattleMaster> fsm, bool isShutdown)
        {
            base.OnLeave(fsm, isShutdown);
            SceneMgr.Instance.ManualCompleteLoad();
        }

        private void InitMap(IFsm<BattleMaster> fsm)
        {
            _mapComplete = false;
            _episodeInfo = ProcedureMgr.Instance.GetData<EpisodeInfo>(BattleConst.MAP_KEY);
            ResMgr.Instance.LoadAsync<GameObject>(GameConst.BATTLE_SCENE, ResPath.GetMapObj(_episodeInfo.id), obj =>
            {
                _episode = obj.GetComponent<EpisodeEntity>();
                if (_episode == null)
                    throw new Exception($"{obj.name} need episode entity.");

                fsm.Owner.shelterPos = new Transform[_episode.shelterPos.Count];
                for (int i = 0; i < _episode.shelterPos.Count; i++)
                    fsm.Owner.shelterPos[i] = _episode.shelterPos[i];
                
                fsm.Owner.flagEntitys = new FlagEntity[_episode.flagPos.Count];
                for (int i = 0; i < _episode.flagPos.Count; i++)
                    fsm.Owner.flagEntitys[i] = _episode.flagPos[i];
                _mapComplete = true;
            });
        }

        private void InitStudents(IFsm<BattleMaster> fsm)
        {
            _studentComplete = false;
            //生成学生
            var formationItem = PlayerMgr.Instance.CurFormation;
            var studentPos = _episode.studentPos;
            int formationCount = formationItem.students.Length;
            if (studentPos.Count < formationCount)
                throw new Exception("Students need more born pos.");

            fsm.Owner.studentEntitys = new BattleStudentEntity[_episode.studentPos.Count];
            int completeNum = 0;
            for (int i = 0; i < formationCount; i++)
            {
                int index = i;
                var student = formationItem.students[i];
                var info = RoleMgr.Instance.GetBattleRole(student.roleId);
                ResMgr.Instance.LoadAsync<GameObject>(GameConst.BATTLE_SCENE, ResPath.GetStudentObj(info), obj =>
                {
                    //设置位置
                    obj.transform.position = studentPos[index].position;
                    obj.transform.rotation = studentPos[index].rotation;

                    //添加战斗逻辑脚本
                    var entity = obj.AddComponent<BattleStudentEntity>();
                    entity.SetStudentInfo(student);
                    entity.InitFsm(index);
                    fsm.Owner.studentEntitys[index] = entity;
                    completeNum++;
                    if (completeNum >= formationCount) _studentComplete = true;
                });
            }
        }

        private void InitEnemy(IFsm<BattleMaster> fsm)
        {
            _enemyComplete = false;

            var enemyPos = _episode.enemyPos;
            var formationCount = _episodeInfo.enemy.Length;
            if (enemyPos.Count < formationCount)
                throw new Exception("Enemys need more born pos.");
            
            fsm.Owner.enemyEntitys = new BattleEnemyEntity[_episode.enemyPos.Count];
            var enemyDic = BinaryDataMgr.Instance.GetTable<EnemyInfoContainer>().dataDic;
            int completeNum = 0;
            for (int i = 0; i < formationCount; i++)
            {
                int index = i;
                var enemyInfo = enemyDic[_episodeInfo.enemy[index]];
                ResMgr.Instance.LoadAsync<GameObject>(GameConst.BATTLE_SCENE, ResPath.GetEnemyObj(enemyInfo.roleName),
                    obj =>
                    {
                        //设置位置
                        obj.transform.position = enemyPos[index].position;
                        obj.transform.rotation = enemyPos[index].rotation;

                        //添加战斗逻辑脚本
                        var entity = obj.AddComponent<BattleEnemyEntity>();
                        entity.SetEnemyInfo(enemyInfo);
                        entity.InitFsm(index);
                        //todo 可以配表设置
                        if (enemyInfo.roleName == "Drone")
                        {
                            entity.cc.center = Vector3.up * 1.9f;
                            entity.cc.radius = 0.75f;
                            entity.cc.height = 1.8f;
                        }
                        else
                        {
                            entity.cc.center = Vector3.up * 1.08f;
                            entity.cc.radius = 0.75f;
                            entity.cc.height = 2f;
                        }
                        fsm.Owner.enemyEntitys[index] = entity;
                        completeNum++;
                        if (completeNum >= formationCount) _enemyComplete = true;
                    });
            }
        }

        private void InitPanel(IFsm<BattleMaster> fsm)
        {
            UIMgr.Instance.ShowPanel<BattlePanel>
                (callback: panel => { fsm.Owner.BattlePanel = panel as BattlePanel; });
            UIMgr.Instance.ShowPanel<BattleRoundPanel>
                (callback: panel => { fsm.Owner.BattleRoundPanel = panel as BattleRoundPanel; });
        }
    }
}
