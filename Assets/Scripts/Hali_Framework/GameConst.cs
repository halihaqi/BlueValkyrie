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
        public const string UIGROUP_BOT = "Bottom";
        public const string UIGROUP_MID = "Middle";
        public const string UIGROUP_TOP = "Top";
        public const string UIGROUP_SYS = "System";
    }
}