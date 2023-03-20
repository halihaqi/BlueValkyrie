using UnityEngine;

namespace Hali_Framework
{
    public static class GameConst
    {
        //基础类型字节长度
        public const int INT_SIZE = sizeof(int);
        public const int LONG_SIZE = sizeof(long);
        public const int FLOAT_SIZE = sizeof(float);
        public const int BOOL_SIZE = sizeof(bool);
        
        //路径
        public static string DATA_BINARY_PATH = $"{Application.streamingAssetsPath}/Binary/";//Excel生成二进制数据文件夹
        
        //数据密钥
        public const byte KEY = 233;
        
        //UI
        //UI组名
        public const string UIGROUP_WORLD = "World";
        public const string UIGROUP_PANEL = "Panel";
        public const string UIGROUP_POP = "Pop";
        public const string UIGROUP_TIP = "Tip";
        public const string UIGROUP_SYS = "System";
        
        //Res
        public const string RES_GROUP_BEGIN = "Begin";
        public const string RES_GROUP_GAME = "Game";
        public const string RES_GROUP_UI = "Ui";
        
        //Layer
        public const string ROLE_LAYER = "Role";
        
        //Data
        public const int FILE_NUM = 20;//存档数
        public const string DATA_PART_PLAYER = "Player";
        public const string DATA_PART_NPC = "NPC";
        
        //Scene
        public const string BEGIN_SCENE = "BeginScene";
        public const string GAME_SCENE = "GameScene";
        public const string BATTLE_SCENE = "BattleScene";
        
        //Tag
        public const string PLAYER_TAG = "Player";
        public const string STUDENT_TAG = "Student";
        public const string ENEMY_TAG = "Enemy";
        public const string SHELTER_TAG = "Shelter";
    }
}