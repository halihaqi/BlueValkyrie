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
            //隐藏所有界面
            UIMgr.Instance.HideAllLoadingPanels();
            UIMgr.Instance.HideAllLoadedPanels();
            
            //设置Player数据
            var player = ProcedureMgr.Instance.GetData<PlayerInfo>(PlayerMgr.PLAYER_DATA_KEY);
            PlayerMgr.Instance.CurPlayer = player;
            
            //进入游戏场景
            SceneMgr.Instance.LoadSceneWithPanel<LoadingPanel>(GameConst.GAME_SCENE, OnEnterScene);
        }

        private void OnEnterScene()
        {
            PlayerMgr.Instance.SetPlayerPrefab(Camera.main);
        }
    }
}