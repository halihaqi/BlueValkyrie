using System.Collections.Generic;
using System.Linq;
using Hali_Framework;

namespace Game.Managers
{
    public class RoleMgr : Singleton<RoleMgr>, IModule
    {
        private Dictionary<int, RoleInfo> _roleDic;
        private Dictionary<int, BattleRoleInfo> _battleRoleDic;

        public int Priority => 2;

        void IModule.Init()
        {
            _roleDic = BinaryDataMgr.Instance.GetTable<RoleInfoContainer>().dataDic;
            _battleRoleDic = BinaryDataMgr.Instance.GetTable<BattleRoleInfoContainer>().dataDic;
        }

        void IModule.Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        void IModule.Dispose()
        {
            _roleDic.Clear();
            _battleRoleDic.Clear();
            _roleDic = null;
            _battleRoleDic = null;
        }

        public RoleInfo GetRole(int roleId)
        {
            return _roleDic.TryGetValue(roleId, out var role) ? role : null;
        }

        public BattleRoleInfo GetBattleRole(int roleId)
        {
            return _battleRoleDic.TryGetValue(roleId, out var role) ? role : null;
        }
        
        public BattleRoleInfo GetBattleRole(RoleInfo info)
        {
            return _battleRoleDic.TryGetValue(info.id, out var role) ? role : null;
        }

        public List<BattleRoleInfo> GetBattleRoles()
        {
            return _battleRoleDic.Values.ToList();
        }

        public int GetRolePieceId(int roleId)
        {
            return BinaryDataMgr.Instance.GetInfo<RolePieceInfoContainer, int, RolePieceInfo>(roleId).pieceId;
        }
    }
}