using Game.Managers;
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
            RoleInfo info = BinaryDataMgr.Instance.GetInfo<RoleInfoContainer, int, RoleInfo>(id);
            return GetSchoolBadgeIcon(info);
        }

        public static string GetStudentIcon(int roleId)
        {
            RoleInfo info = BinaryDataMgr.Instance.GetInfo<RoleInfoContainer, int, RoleInfo>(roleId);
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
        
        public static string GetItemIcon(int itemId)
        {
            var info = ItemMgr.Instance.GetItem(itemId);
            return GetItemIcon(info);
        }

        public static string GetItemIcon(ItemInfo info)
        {
            return $"Image/ItemIcon/{((ItemType)info.type).ToString()}/{info.resName}";
        }
    }
}