using System.IO;
using Game.Managers;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class ClearSave
    {
        [MenuItem("Tools/Saves/清空数据")]
        public static void ClearAllSave()
        {
            ClearFilesAndDirs(Application.persistentDataPath);
        }

        [MenuItem("Tools/Saves/清除玩家存档")]
        public static void ClearPlayerData()
        {
            ClearFilesAndDirs($"{Application.persistentDataPath}/{PlayerMgr.PLAYER_DATA_KEY}");
        }

        private static void ClearFilesAndDirs(string parentPath)
        {
            if (!Directory.Exists(parentPath)) return;
            
            foreach (string file in Directory.GetFiles(parentPath))
            {
                File.Delete(file);
            }
            foreach (string subFolder in Directory.GetDirectories(parentPath))
            {
                ClearFilesAndDirs(subFolder);
            }
            Directory.Delete(parentPath);
        }
    }
}