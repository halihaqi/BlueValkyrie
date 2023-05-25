using System.Collections;
using System.Collections.Generic;
using Game.BattleScene.BattleEnemyFsm;
using Game.BattleScene.BattleStudentFsm;
using Game.Entity;
using Game.UI.Battle;
using Game.UI.Controls.Battle;
using Hali_Framework;
using UnityEngine;

namespace Game.BattleScene
{
    public class BattleArrayState : FsmState<BattleMaster>
    {
        private BattleMaster _bm;
        private IFsm<BattleMaster> _fsm;
        
        //Chess
        private bool _isGenerateChessOver = false;
        private List<GameObject> studentChesses = new List<GameObject>();
        private List<GameObject> enemyChesses = new List<GameObject>();
        private List<GameObject> flagChesses = new List<GameObject>();
        private const string STUDENT_CHESS_PATH = "UI/Controls/student_chess";
        private const string ENEMY_CHESS_PATH = "UI/Controls/enemy_chess";
        private const string FLAG_CHESS_PATH = "UI/Controls/flag_chess";

        protected internal override void OnEnter(IFsm<BattleMaster> fsm)
        {
            base.OnEnter(fsm);
            _bm = fsm.Owner;
            _fsm = fsm;
            EventMgr.Instance.AddListener<BattleRoleEntity>(ClientEvent.BATTLE_ROLE_ACTION, OnRoleAction);
            EventMgr.Instance.AddListener<BattleRoleEntity>(ClientEvent.BATTLE_ROLE_REST, OnRoleRest);

            UIMgr.Instance.RefocusPanel(_bm.BattleRoundPanel, null);
            MonoMgr.Instance.StartCoroutine(GenerateChess(fsm.Owner, _bm.BattleRoundPanel));
        }

        protected internal override void OnLeave(IFsm<BattleMaster> fsm, bool isShutdown)
        {
            base.OnLeave(fsm, isShutdown);
            RecycleChess();
            EventMgr.Instance.RemoveListener<BattleRoleEntity>(ClientEvent.BATTLE_ROLE_ACTION, OnRoleAction);
            EventMgr.Instance.RemoveListener<BattleRoleEntity>(ClientEvent.BATTLE_ROLE_REST, OnRoleRest);
        }

        protected internal override void OnDestroy(IFsm<BattleMaster> fsm)
        {
            base.OnDestroy(fsm);
            DisposeChess();
        }

        private void OnRoleAction(BattleRoleEntity role)
        {
            if(role is BattleEnemyEntity enemy)
                _bm.SwitchEnemy(enemy);
            else if(role is BattleStudentEntity student)
                _bm.SwitchStudent(student);
            ChangeState<BattleRunState>(_fsm);
        }

        private void OnRoleRest(BattleRoleEntity role)
        {
            if (role is BattleEnemyEntity enemy)
            {
                _bm.SwitchEnemy(enemy);
                enemy.Fsm.ChangeState<EnemyRestState>();
                enemy.ResetAmmo();
                enemy.anim.SetTrigger(BattleEnemyEntity.BattleReload);
            }
            else if (role is BattleStudentEntity student)
            {
                _bm.SwitchStudent(student);
                student.Fsm.ChangeState<StudentRestState>();
                student.ResetAmmo();
                student.anim.SetTrigger(BattleStudentEntity.BattleReload);
            }
            ChangeState<BattleRunState>(_fsm);
            DelayUtils.Instance.Delay(2,1, obj =>
            {
                ChangeState<BattleArrayState>(_fsm);
                EventMgr.Instance.TriggerEvent(ClientEvent.BATTLE_STEP_OVER);
            });
        }

        private IEnumerator GenerateChess(BattleMaster bm, BattleRoundPanel panel)
        {
            _isGenerateChessOver = false;
            var map = panel.Map;
            var parent = panel.ChessTrans;
            Dictionary<int, Vector2> chessPosDic;
            Dictionary<int, float> chessRotationDic;
            float camOffset = bm.mapCam.transform.eulerAngles.y;
            int completeNum = 0;

            //生成旗帜棋子
            chessPosDic = bm.CalcFlagMapPos(map);
            foreach (var kv in chessPosDic)
            {
                ObjectPoolMgr.Instance.PopObj(FLAG_CHESS_PATH, obj =>
                {
                    var item = obj.GetComponent<UI_flag_chess>();
                    item.OnInit();
                    item.SetData(bm.flags[kv.Key].FlagType);
                    var trans = (RectTransform)item.transform;
                    trans.SetParent(parent, false);
                    trans.anchoredPosition = kv.Value;
                    flagChesses.Add(obj);
                    ++completeNum;
                });
            }

            while (completeNum < chessPosDic.Count)
            {
                yield return null;
            }
            
            //生成我方棋子
            chessPosDic = bm.CalcRolesMapPos(false, map);
            chessRotationDic = bm.CalcRolesMapRotation(false);
            completeNum = 0;
            foreach (var kv in chessPosDic)
            {
                ObjectPoolMgr.Instance.PopObj(STUDENT_CHESS_PATH, obj =>
                {
                    var item = obj.GetComponent<UI_battle_chess>();
                    item.OnInit();
                    item.SetRole(bm.GetStudent(kv.Key));
                    var trans = (RectTransform)item.transform;
                    trans.SetParent(parent, false);
                    trans.anchoredPosition = kv.Value;
                    item.rotRect.localRotation = Quaternion.Euler(0, 0, chessRotationDic[kv.Key] + camOffset);
                    studentChesses.Add(obj);
                    ++completeNum;
                });
            }

            while (completeNum < chessPosDic.Count)
            {
                yield return null;
            }
            
            //生成敌方棋子
            chessPosDic = bm.CalcRolesMapPos(true, map);
            chessRotationDic = bm.CalcRolesMapRotation(true);
            completeNum = 0;
            foreach (var kv in chessPosDic)
            {
                ObjectPoolMgr.Instance.PopObj(ENEMY_CHESS_PATH, obj =>
                {
                    var item = obj.GetComponent<UI_battle_chess>();
                    item.OnInit();
                    item.SetRole(bm.GetEnemy(kv.Key));
                    var trans = (RectTransform)item.transform;
                    trans.SetParent(parent, false);
                    trans.anchoredPosition = kv.Value;
                    item.rotRect.localRotation = Quaternion.Euler(0, 0, chessRotationDic[kv.Key] + camOffset);
                    enemyChesses.Add(obj);
                    ++completeNum;
                });
            }
            
            while (completeNum < chessPosDic.Count)
            {
                yield return null;
            }
            
            _isGenerateChessOver = true;
        }

        private void RecycleChess()
        {
            foreach (var chess in studentChesses)
                chess.GetComponent<UI_battle_chess>().OnRecycle();
            foreach (var chess in enemyChesses)
                chess.GetComponent<UI_battle_chess>().OnRecycle();
            foreach (var chess in flagChesses)
                chess.GetComponent<UI_flag_chess>().OnRecycle();
            ObjectPoolMgr.Instance.PushObjs(STUDENT_CHESS_PATH, studentChesses);
            ObjectPoolMgr.Instance.PushObjs(ENEMY_CHESS_PATH, enemyChesses);
            ObjectPoolMgr.Instance.PushObjs(FLAG_CHESS_PATH, flagChesses);
            studentChesses.Clear();
            enemyChesses.Clear();
            flagChesses.Clear();
        }

        private void DisposeChess()
        {
            foreach (var chess in studentChesses)
                chess.GetComponent<UI_battle_chess>().OnRecycle();
            foreach (var chess in enemyChesses)
                chess.GetComponent<UI_battle_chess>().OnRecycle();
            foreach (var chess in flagChesses)
                chess.GetComponent<UI_flag_chess>().OnRecycle();
            ObjectPoolMgr.Instance.ClearPool(STUDENT_CHESS_PATH);
            ObjectPoolMgr.Instance.ClearPool(ENEMY_CHESS_PATH);
            ObjectPoolMgr.Instance.ClearPool(FLAG_CHESS_PATH);
            studentChesses.Clear();
            enemyChesses.Clear();
            flagChesses.Clear();
        }
    }
}