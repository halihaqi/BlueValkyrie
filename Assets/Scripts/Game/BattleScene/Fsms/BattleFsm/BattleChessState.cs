using System.Collections;
using System.Collections.Generic;
using Game.BattleScene.BattleRole;
using Game.Entity;
using Game.UI.Battle;
using Game.UI.Controls;
using Hali_Framework;
using UnityEngine;

namespace Game.BattleScene
{
    public class BattleChessState : FsmState<BattleMaster>
    {
        private BattleMaster _bm;
        private IFsm<BattleMaster> _fsm;
        
        //Chess
        private bool _isGenerateChessOver = false;
        private List<GameObject> roleChess = new List<GameObject>();
        private List<GameObject> flagChess = new List<GameObject>();
        private const string ROLE_CHESS_PATH = "UI/Controls/role_chess";
        private const string FLAG_CHESS_PATH = "UI/Controls/flag_chess";

        protected internal override void OnEnter(IFsm<BattleMaster> fsm)
        {
            base.OnEnter(fsm);
            _bm = fsm.Owner;
            _fsm = fsm;
            EventMgr.Instance.AddListener<IBattleRole>(ClientEvent.BATTLE_ROLE_ACTION, OnRoleAction);
            EventMgr.Instance.AddListener<IBattleRole>(ClientEvent.BATTLE_ROLE_REST, OnRoleRest);

            UIMgr.Instance.RefocusPanel(_bm.BattleRoundPanel, null);
            MonoMgr.Instance.StartCoroutine(PopChess(fsm.Owner, _bm.BattleRoundPanel));
        }

        protected internal override void OnLeave(IFsm<BattleMaster> fsm, bool isShutdown)
        {
            base.OnLeave(fsm, isShutdown);
            PushChess();
            EventMgr.Instance.RemoveListener<IBattleRole>(ClientEvent.BATTLE_ROLE_ACTION, OnRoleAction);
            EventMgr.Instance.RemoveListener<IBattleRole>(ClientEvent.BATTLE_ROLE_REST, OnRoleRest);
        }

        protected internal override void OnDestroy(IFsm<BattleMaster> fsm)
        {
            base.OnDestroy(fsm);
            DisposeChess();
        }

        private void OnRoleAction(IBattleRole role)
        {
            _bm.ChangeRole(role);
            ChangeState<BattleRunState>(_fsm);
        }

        private void OnRoleRest(IBattleRole role)
        {
            _bm.ChangeRole(role);
            ChangeState<BattleRunState>(_fsm);
            DelayUtils.Instance.Delay(2,1, obj =>
            {
                ChangeState<BattleChessState>(_fsm);
                EventMgr.Instance.TriggerEvent(ClientEvent.BATTLE_STEP_OVER);
            });
        }

        private IEnumerator PopChess(BattleMaster bm, BattleRoundPanel panel)
        {
            _isGenerateChessOver = false;
            var map = panel.Map;
            var parent = panel.ChessTrans;
            float camOffset = bm.mapCam.transform.eulerAngles.y;
            Dictionary<FlagEntity, Vector2> flagStation;
            Dictionary<IBattleRole, Vector2> roleStation;
            Dictionary<IBattleRole, float> roleRotation;

            foreach (var type in bm.CampTypes)
            {
                //生成旗帜棋子
                flagStation = bm.GetFlagChessStation(type, map);
                foreach (var kv in flagStation)
                {
                    bool loaded = false; 
                    ObjectPoolMgr.Instance.PopObj(FLAG_CHESS_PATH, obj =>
                    {
                        var item = obj.GetComponent<UI_flag_chess>();
                        item.OnInit();
                        item.SetData(kv.Key.FlagType);
                        var trans = (RectTransform)item.transform;
                        trans.SetParent(parent, false);
                        trans.anchoredPosition = kv.Value;
                        flagChess.Add(obj);
                        loaded = true;
                    });
                    while(!loaded)
                        yield return null;
                }
                
                //生成士兵棋子
                roleStation = bm.GetRoleChessStation(type, map);
                roleRotation = bm.GetRoleChessRotation(type);
                foreach (var kv in roleStation)
                {
                    bool loaded = false; 
                    ObjectPoolMgr.Instance.PopObj(ROLE_CHESS_PATH, obj =>
                    {
                        // var item = obj.GetComponent<UI_battle_chess>();
                        // item.OnInit();
                        // item.SetData(kv.Key);
                        // var trans = (RectTransform)item.transform;
                        // trans.SetParent(parent, false);
                        // trans.anchoredPosition = kv.Value;
                        // item.rotRect.localRotation = Quaternion.Euler(0, 0, roleRotation[kv.Key] + camOffset);
                        // roleChess.Add(obj);
                        // loaded = true;
                    });
                    while(!loaded)
                        yield return null;
                }
            }
            
            _isGenerateChessOver = true;
        }

        private void PushChess()
        {
            // foreach (var chess in roleChess)
            //     chess.GetComponent<UI_battle_chess>().OnRecycle();
            foreach (var chess in flagChess)
                chess.GetComponent<UI_flag_chess>().OnRecycle();
            ObjectPoolMgr.Instance.PushObjs(ROLE_CHESS_PATH, roleChess);
            ObjectPoolMgr.Instance.PushObjs(FLAG_CHESS_PATH, flagChess);
            roleChess.Clear();
            flagChess.Clear();
        }

        private void DisposeChess()
        {
            // foreach (var chess in roleChess)
            //     chess.GetComponent<UI_battle_chess>().OnRecycle();
            foreach (var chess in flagChess)
                chess.GetComponent<UI_flag_chess>().OnRecycle();
            ObjectPoolMgr.Instance.ClearPool(ROLE_CHESS_PATH);
            ObjectPoolMgr.Instance.ClearPool(FLAG_CHESS_PATH);
            roleChess.Clear();
            flagChess.Clear();
        }
    }
}