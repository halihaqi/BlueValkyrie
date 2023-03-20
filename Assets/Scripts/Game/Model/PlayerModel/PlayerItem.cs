using System;
using System.Collections.Generic;
using Game.Managers;
using Game.Model.BagModel;
using Game.Model.BattleModel;
using Game.Model.EpisodeModel;

namespace Game.Model
{
    [System.Serializable]
    public class PlayerItem : IBagRole
    {
        public int id;
        public string name;
        public float time;
        public float complete;//完成度 [0,1]
        public string school;

        public int secretaryId;

        public Inventory Inventory { get; set; }
        
        public ShopItem ShopItem { get; set; }

        public Dictionary<int, StudentItem> Students { get; set; }
        
        public Dictionary<int, EpisodeItem> Episodes { get; set; }
        
        public List<FormationItem> Formation { get; set; }

        public PlayerItem(){}
        public PlayerItem(int id, string name, int secretaryId)
        {
            this.id = id;
            this.name = name;
            this.secretaryId = secretaryId;
            time = 0;
            complete = 0;
            
            //仓库
            Inventory = new Inventory(id);
            Inventory.AddBag(0);//Player默认存在编号为0的背包
            Inventory.AddItem(0, 1, 1000);//初始金币
            Inventory.AddItem(0, 2, 300);//初始钻石
            //todo Test 先添加所有道具
            var list = ItemMgr.Instance.GetItemIds();
            foreach (var itemId in list)
            {
                Inventory.AddItem(0, itemId, 100);
            }
            
            //学生
            ShopItem = new ShopItem(id);
            Students = new Dictionary<int, StudentItem>();
            
            //todo 暂时先添加所有战斗学生
            var students = RoleMgr.Instance.GetBattleRoles();
            foreach (var student in students)
            {
                JoinStudent(new StudentItem(student.id));
            }
            
            //关卡
            Episodes = new Dictionary<int, EpisodeItem>(EpisodeMgr.Instance.EpisodeCount);
            var episodes = EpisodeMgr.Instance.GetAllEpisodeInfo();
            foreach (var info in episodes)
            {
                Episodes.Add(info.id, new EpisodeItem(info.id));
            }
            
            //队伍
            Formation = new List<FormationItem>(4);
            for (int i = 0; i < 4; i++)
            {
                Formation.Add(new FormationItem(i + 1));
            }
            
            //todo 暂时先添加4个默认学生
            int index = 0;
            foreach (var item in Formation)
            {
                for (int i = 0; i < item.students.Length; i++)
                {
                    item.students[i] = Students[students[index++].id];
                    if(index >= students.Count) return;
                }
            }
        }

        public void JoinStudent(StudentItem student)
        {
            if (!Students.ContainsKey(student.roleId))
                Students.Add(student.roleId, student);
            else
                throw new Exception($"Already has student who's id is {student.roleId}.");
        }
    }
}