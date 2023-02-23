using Game.Managers;
using Game.Model;
using Game.UI.Base;
using Hali_Framework;

namespace Game.Global
{
    public class GameProcedure : ProcedureBase
    {
        protected internal override void OnEnter(IFsm<ProcedureMgr> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            //隐藏所有界面
            UIMgr.Instance.HideAllLoadedPanels();
            UIMgr.Instance.HideAllLoadingPanels();
            
            //设置Player数据
            var player = ProcedureMgr.Instance.GetData<PlayerInfo>(PlayerMgr.PLAYER_DATA_KEY);
            PlayerMgr.Instance.NowPlayer = player;
            
            //进入游戏场景
            SceneMgr.Instance.LoadSceneWithPanel<LoadingPanel>(GameConst.GAME_SCENE, OnEnterScene);
        }

        private void OnEnterScene()
        {
            
        }
    }
}