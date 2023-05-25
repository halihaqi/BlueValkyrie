using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Game.BattleScene.BattleRole;
using Game.Entity;
using Game.Managers;
using Game.UI.Battle;
using Game.Utils;
using Hali_Framework;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Game.BattleScene
{
    public class BattleInitState : FsmState<BattleMaster>
    {
        private Stopwatch _sw;
        private bool _initComplete;

        private EpisodeEntity _episode;
        private EpisodeInfo _episodeInfo;
        
        protected internal override void OnEnter(IFsm<BattleMaster> fsm)
        {
            base.OnEnter(fsm);
            MonoMgr.Instance.StartCoroutine(InitBattlefield(fsm));
            Debug.Log("<battle> 开始加载战场");
            _sw = Stopwatch.StartNew();
        }

        protected internal override void OnUpdate(IFsm<BattleMaster> fsm, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(fsm, elapseSeconds, realElapseSeconds);
            if (_initComplete)
            {
                Debug.Log($"<battle> 战场加载完成，耗时：{_sw.ElapsedMilliseconds} ms");
                ChangeState<BattleStartState>(fsm);
            }
        }

        protected internal override void OnLeave(IFsm<BattleMaster> fsm, bool isShutdown)
        {
            base.OnLeave(fsm, isShutdown);
            _sw.Stop();
            _sw = null;
            SceneMgr.Instance.ManualCompleteLoad();
        }

        private IEnumerator InitBattlefield(IFsm<BattleMaster> fsm)
        {
            //1.先加载地图
            bool mapComplete = false;
            var episodeInfo = ProcedureMgr.Instance.GetData<EpisodeInfo>(BattleConst.MAP_KEY);
            EpisodeEntity episodeEntity = null;
            ResMgr.Instance.LoadAsync<GameObject>(GameConst.BATTLE_SCENE, ResPath.GetMapObj(_episodeInfo.id), obj =>
            {
                episodeEntity = obj.GetComponent<EpisodeEntity>();
                if (episodeEntity == null)
                    throw new Exception($"{obj.name} need episode entity.");
                mapComplete = true;
            });
            while (!mapComplete)
                yield return null;
            
            //2.再加载战斗角色
            var camps = episodeEntity.campPos;
            foreach (var camp in camps)
            {
                CampInfo info = new CampInfo();
                info.campType = camp.type;
                info.flags = camp.flags;
                //加载士兵
                List<IBattleRole> roles = new List<IBattleRole>();
                yield return MonoMgr.Instance.StartCoroutine
                    (BattleLoadHelper.LoadSolider(camp, roles));
                //todo 设置队长
                info.soldiers = roles;
                fsm.Owner.AddCamp(info);
            }

            //3.最后加载面板
            InitPanel(fsm);
        }

        private void InitEnemy(IFsm<BattleMaster> fsm)
        {
            _enemyComplete = false;

            var enemyPos = _episode.enemyPos;
            var formationCount = _episodeInfo.enemy.Length;
            if (enemyPos.Count < formationCount)
                throw new Exception("Enemys need more born pos.");
            
            fsm.Owner.enemies = new BattleEnemyEntity[_episode.enemyPos.Count];
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
                        fsm.Owner.enemies[index] = entity;
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
