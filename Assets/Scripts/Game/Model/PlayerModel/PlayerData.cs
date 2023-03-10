using System.Collections.Generic;
using Hali_Framework;

namespace Game.Model
{
    [System.Serializable]
    public class PlayerData
    {
        public Dictionary<int, PlayerItem> dataDic;

        public PlayerData()
        {
            dataDic = new Dictionary<int, PlayerItem>(GameConst.FILE_NUM);
        }
    }
}