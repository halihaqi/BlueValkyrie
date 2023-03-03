using System.Collections.Generic;
using Game.Model.BagModel;
using Hali_Framework;

namespace Game.Model
{
    [System.Serializable]
    public class PlayerData
    {
        public Dictionary<int, PlayerInfo> dataDic;

        public PlayerData()
        {
            dataDic = new Dictionary<int, PlayerInfo>(GameConst.FILE_NUM);
        }
    }
    
    [System.Serializable]
    public class PlayerInfo : IBagRole
    {
        public int id;
        public string name;
        public int time;
        public float complete;//完成度 [0,1]

        public int secretaryId;

        public BagData BagData { get; set; }
        
        public ShopInfo ShopInfo { get; set; }

        public PlayerInfo(){}
        public PlayerInfo(int id, string name, int secretaryId)
        {
            this.id = id;
            this.name = name;
            this.secretaryId = secretaryId;
            time = 0;
            complete = 0;
            BagData = new BagData(id);
            BagData.AddBag(0);//Player默认存在编号为0的背包
            ShopInfo = new ShopInfo(id);
        }
    }
}