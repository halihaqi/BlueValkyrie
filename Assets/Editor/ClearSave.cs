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
            var files = Directory.GetFiles(Application.persistentDataPath);
            for (int i = 0; i < files.Length; i++)
            {
                File.Delete(files[i]);
            }
        }

        [MenuItem("Tools/Saves/清除玩家存档")]
        public static void ClearPlayerData()
        {
            var files = Directory.GetFiles($"{Application.persistentDataPath}/{PlayerMgr.PLAYER_DATA_KEY}");
            for (int i = 0; i < files.Length; i++)
            {
                File.Delete(files[i]);
            }
        }
    }
}