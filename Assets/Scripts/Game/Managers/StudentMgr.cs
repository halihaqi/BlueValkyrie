using System.Collections.Generic;
using System.Linq;
using Hali_Framework;

namespace Game.Managers
{
    public class StudentMgr : Singleton<StudentMgr>, IModule
    {
        private List<RoleLevelInfo> _lvlUpList;
        private List<RoleStarInfo> _starUpList;

        public int MaxLvl => _lvlUpList.Count;

        public int MaxStar => _starUpList.Count;

        public int Priority => 2;
        void IModule.Init()
        {
            _lvlUpList = BinaryDataMgr.Instance.GetTable<RoleLevelInfoContainer>().dataDic.Values.ToList();
            _starUpList = BinaryDataMgr.Instance.GetTable<RoleStarInfoContainer>().dataDic.Values.ToList();
        }

        void IModule.Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        void IModule.Dispose()
        {
            _lvlUpList.Clear();
            _starUpList.Clear();
            _lvlUpList = null;
            _starUpList = null;
        }

        /// <summary>
        /// 获取某级升到下一级所需经验，如果超出等级，返回-1
        /// </summary>
        /// <param name="lvl"></param>
        /// <returns></returns>
        public int GetLvlUpExp(int lvl)
        {
            var info = _lvlUpList.Find(o => o.lv == lvl);
            if(info != null)
                return info.exp;
            return -1;
        }

        
        public List<RoleLevelInfo> GetLevelUpList() => _lvlUpList;

        public List<RoleStarInfo> GetStarUpList() => _starUpList;

        public RoleLevelInfo GetLvlUpInfo(int lvl) => _lvlUpList[lvl - 1];
        
        public RoleStarInfo GetStarUpInfo(int star) => _starUpList[star - 1];
    }
}