using System.Collections.Generic;
using System.Linq;
using Hali_Framework;

namespace Game.Managers
{
    public class EpisodeMgr : Singleton<EpisodeMgr>, IModule
    {
        private Dictionary<int, EpisodeInfo> _episodeDic;

        public int EpisodeCount => _episodeDic.Count;

        public int Priority => 2;
        void IModule.Init()
        {
            _episodeDic = BinaryDataMgr.Instance.GetTable<EpisodeInfoContainer>().dataDic;
        }

        void IModule.Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        void IModule.Dispose()
        {
        }

        public EpisodeInfo GetEpisodeInfo(int id)
        {
            if (_episodeDic.TryGetValue(id, out var info))
                return info;

            return null;
        }

        public List<EpisodeInfo> GetAllEpisodeInfo()
        {
            return _episodeDic.Values.ToList();
        }
    }
}