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

        protected internal override void OnEnter(IFsm<BattleMaster> fsm)
        {
            base.OnEnter(fsm);
            Debug.Log("<battle> 开始加载战场");
            _sw = Stopwatch.StartNew();
            MonoMgr.Instance.StartCoroutine(InitBattlefield(fsm));
        }

        protected internal override void OnUpdate(IFsm<BattleMaster> fsm, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(fsm, elapseSeconds, realElapseSeconds);
            if (_initComplete)
            {
                Debug.Log($"<battle> 战场加载完成，耗时：{_sw.ElapsedMilliseconds} ms");
                _sw.Stop();
                _sw = null;
                ChangeState<BattleStartState>(fsm);
            }
        }

        protected internal override void OnLeave(IFsm<BattleMaster> fsm, bool isShutdown)
        {
            base.OnLeave(fsm, isShutdown);
            SceneMgr.Instance.ManualCompleteLoad();
        }

        private IEnumerator InitBattlefield(IFsm<BattleMaster> fsm)
        {
            //1.先加载地图
            bool mapComplete = false;
            var episodeInfo = ProcedureMgr.Instance.GetData<EpisodeInfo>(BattleConst.MAP_KEY);
            EpisodeEntity episodeEntity = null;
            ResMgr.Instance.LoadAsync<GameObject>(GameConst.BATTLE_SCENE, ResPath.GetMapObj(episodeInfo.id), obj =>
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
                fsm.Owner.JoinCamp(info);
            }

            int panelLoaded = 0;
            //3.最后加载面板
            UIMgr.Instance.ShowPanel<BattlePanel>
                (callback: panel =>
                {
                    fsm.Owner.BattlePanel = panel as BattlePanel;
                    panelLoaded++;
                });
            UIMgr.Instance.ShowPanel<BattleRoundPanel>
                (callback: panel =>
                {
                    fsm.Owner.BattleRoundPanel = panel as BattleRoundPanel;
                    panelLoaded++;
                });
            while (panelLoaded != 2)
                yield return null;
            //加载完成
            _initComplete = true;
        }
    }
}
