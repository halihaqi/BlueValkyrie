using System.Collections.Generic;
using Hali_Framework;

namespace Game.Model
{
    [System.Serializable]
    public class PlayerData
    {
        public Dictionary<int, PlayerInfo> dataDic;

        public PlayerData()
        {
            dataDic = new (GameConst.FILE_NUM);
            for (int i = 0; i < GameConst.FILE_NUM; i++)
            {
                dataDic.Add(i, null);
            }
        }
    }
    
    [System.Serializable]
    public class PlayerInfo
    {
        public int id;
        public string name;
        public int time;
        public float complete;//完成度 [0,1]
    }
}