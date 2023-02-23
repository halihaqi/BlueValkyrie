
using Hali_Framework;

namespace Game.Global
{
    public class GameMaster : SingletonMono<GameMaster>
    {
        protected override void Awake()
        {
            base.Awake();
            //初始化框架
            FrameworkEntry.Init();
            
            //开启流程
            ProcedureMgr.Instance.Initialize(new InitProcedure(), new BeginProcedure(), new GameProcedure());
            ProcedureMgr.Instance.StartProcedure<InitProcedure>();
        }
    }
}