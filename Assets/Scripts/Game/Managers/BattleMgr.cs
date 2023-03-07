using Hali_Framework;

namespace Game.Managers
{
    public class BattleMgr : Singleton<BattleMgr>, IModule
    {
        public int Priority => 3;
        void IModule.Init()
        {
        }

        void IModule.Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        void IModule.Dispose()
        {
        }
    }
}