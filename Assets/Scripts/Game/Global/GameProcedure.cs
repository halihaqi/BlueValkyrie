using Game.Managers;
using Game.Model;
using Game.UI.Base;
using Hali_Framework;
using UnityEngine;

namespace Game.Global
{
    public class GameProcedure : ProcedureBase
    {
        protected internal override void OnEnter(IFsm<ProcedureMgr> procedureOwner)
        {
            base.OnEnter(procedureOwner);

            //设置Player数据
            var player = ProcedureMgr.Instance.GetData<PlayerInfo>(PlayerMgr.PLAYER_DATA_KEY);
            PlayerMgr.Instance.CurPlayer = player;
            
            //进入游戏场景
            SceneMgr.Instance.LoadSceneWithPanel<LoadingPanel>(GameConst.GAME_SCENE, OnEnterScene);
        }

        protected internal override void OnLeave(IFsm<ProcedureMgr> procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            PlayerMgr.Instance.UnloadPlayerEntity();
            //隐藏所有界面
            UIMgr.Instance.HideAllLoadingPanels();
            UIMgr.Instance.HideAllLoadedPanels();
        }

        private void OnEnterScene()
        {
            PlayerMgr.Instance.RegisterPlayerEntity(Camera.main);
        }
    }
}