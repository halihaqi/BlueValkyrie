using Game.Managers;
using Game.Model;
using Game.Model.BagModel;
using Hali_Framework;
using UnityEngine;

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
        
        public static string GetStudentIcon(string roleName)
        {
            return $"Image/HeadIcon/Student_Portrait_{roleName}";
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
            return $"Image/EnemyIcon/EnemyInfo_{enemyName}";
        }

        public static string GetRoleIcon(RoleType type, string roleName)
        {
            switch (type)
            {
                case RoleType.Student:
                    return GetStudentIcon(roleName);
                    break;
                case RoleType.Enemy:
                    return GetEnemyIcon(roleName);
                    break;
                default:
                    return "";
            }
        }

        public static string GetAmmonIcon(RoleType type)
        {
            return $"Image/AmmoIcon/{type}_ammo_icon";
        }

        public static Color GetRoleColor(RoleType type)
        {
            switch (type)
            {
                case RoleType.Student:
                    return new Color(0.4f, 1, 0.9f);
                    break;
                case RoleType.Enemy:
                    return new Color(1, 0.4f, 0.4f);
                    break;
                default:
                    return Color.white;
            }
        }

        public static string GetChessIcon(RoleType type)
        {
            return $"Image/ChessIcon/{type.ToString().ToLower()}_chess_icon";
        }
    }
}