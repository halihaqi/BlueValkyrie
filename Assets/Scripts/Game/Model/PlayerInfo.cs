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
        }
    }
    
    [System.Serializable]
    public class PlayerInfo
    {
        public int id;
        public string name;
        public int time;
        public float complete;//完成度 [0,1]

        public int secretaryId;
    }
}