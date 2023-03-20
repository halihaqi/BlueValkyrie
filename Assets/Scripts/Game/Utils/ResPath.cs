using Game.Managers;
using Game.Model;
using Game.Model.BagModel;
using Hali_Framework;

namespace Game.Utils
{
    public static class ResPath
    {
        public static string GetSchoolBadgeIcon(RoleInfo info)
        {
            string school = info.school.ToUpper();
            return $"Image/SchoolBadge/School_Icon_{school}";
        }
        
        public static string GetSchoolBadgeIcon(int id)
        {
            RoleInfo info = RoleMgr.Instance.GetRole(id);
            return GetSchoolBadgeIcon(info);
        }

        public static string GetStudentIcon(int roleId)
        {
            RoleInfo info = RoleMgr.Instance.GetRole(roleId);
            return GetStudentIcon(info);
        }
        
        public static string GetStudentIcon(RoleInfo info)
        {
            return $"Image/HeadIcon/Student_Portrait_{info.name}";
        }

        public static string GetStudentObj(RoleInfo info)
        {
            return $"Prefabs/Students/{info.school}/{info.name}";
        }
        
        public static string GetStudentObj(BattleRoleInfo info)
        {
            return $"Prefabs/Students/{info.school}/{info.name}";
        }
        
        public static string GetStudentObj(int roleId)
        {
            var role = RoleMgr.Instance.GetRole(roleId);
            if (role == null) return null;
            return $"Prefabs/Students/{role.school}/{role.name}";
        }

        public static string GetItemIcon(int itemId)
        {
            var info = ItemMgr.Instance.GetItem(itemId);
            return GetItemIcon(info);
        }

        public static string GetItemIcon(ItemInfo info)
        {
            return $"Image/ItemIcon/{((ItemType)info.type).ToString()}/{info.resName}";
        }

        public static string GetMiniMap(int episodeId)
        {
            return $"Image/MiniMap/episode_{episodeId}";
        }

        public static string GetMapObj(int episodeId)
        {
            return $"Prefabs/Map/Episode{episodeId / 10 + 1}_{episodeId % 10}";
        }

        public static string GetGunIcon(AtkType type)
        {
            return $"Image/GunIcon/Weapon_Icon_{type.ToString()}";
        }

        public static string GetEnemyObj(string enemyName)
        {
            return $"Prefabs/Enemys/{enemyName}";
        }
        
        public static string GetEnemyIcon(string enemyName)
        {
            return $"Image/Enemy_Icon/EnemyInfo_{enemyName}";
        }
    }
}