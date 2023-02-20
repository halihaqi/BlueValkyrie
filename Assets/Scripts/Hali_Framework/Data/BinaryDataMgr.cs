using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace Hali_Framework
{
    public class BinaryDataMgr : Singleton<BinaryDataMgr>, IModule
    {
        //Excel二进制数据容器，键为数据容器名
        private Dictionary<string, object> _dataDic;

        int IModule.Priority => 0;

        public BinaryDataMgr()
        {
            _dataDic = new Dictionary<string, object>();
            InitTableData();
        }

        void IModule.Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        void IModule.Dispose()
        {
            _dataDic.Clear();
        }
    
        
        public void Save(string part, string name, object data)
        {
            string parentFile = $"{Application.persistentDataPath}/{part}";
            string path = $"{Application.persistentDataPath}/{part}/{name}.zxy";

            if (Directory.Exists(parentFile))
                Directory.CreateDirectory(parentFile);
            
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, data);
                fs.Flush();
                fs.Close();
            }
        }

        public T Load<T>(string part, string name)
        {
            string path = $"{Application.persistentDataPath}/{part}/{name}.zxy";
            if (!File.Exists(path))
                return default;

            T obj;
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                obj = (T)bf.Deserialize(fs);
                fs.Close();
            }

            return obj;
        }

        /// <summary>
        /// 加载Excel表的二进制数据存入数据字典
        /// </summary>
        /// <param name="containerName">容器名</param>
        /// <param name="className">数据结构名</param>
        private void LoadTable(string containerName, string className)
        {
            Type containerType = Type.GetType(containerName);
            Type classType = Type.GetType(className);
            if (containerType == null || classType == null)
            {
                Debug.LogError($"{containerName} 类不存在 或 {className} 类不存在");
                return;
            }
        
            //1.读取二进制数据
            byte[] bytes;
            using (FileStream fs = File.Open($"{GameConst.DATA_BINARY_PATH}{classType.Name}.zxy", FileMode.Open, FileAccess.Read))
            {
                bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                fs.Close();
            }
            var index = 0;
        
            //2.读取数据行数
            int count = BitConverter.ToInt32(bytes, index);
            index += GameConst.INT_SIZE;
            
            //3.读取主键名
            int keyNameLength = BitConverter.ToInt32(bytes, index);
            index += GameConst.INT_SIZE;
            string keyName = Encoding.UTF8.GetString(bytes, index, keyNameLength);
            index += keyNameLength;
            
            //4.读取每一行信息
            //实例化容器类
            object containerObj = Activator.CreateInstance(containerType);
            FieldInfo[] infos = classType.GetFields();
            for (var i = 0; i < count; i++)
            {
                //实例化数据结构类
                object dataObj = Activator.CreateInstance(classType);
                //填充数据结构类
                foreach (FieldInfo info in infos)
                {
                    if (info.FieldType == typeof(int))
                    {
                        info.SetValue(dataObj, BitConverter.ToInt32(bytes, index));
                        index += GameConst.INT_SIZE;
                    }
                    else if (info.FieldType == typeof(long))
                    {
                        info.SetValue(dataObj, BitConverter.ToInt64(bytes, index));
                        index += GameConst.LONG_SIZE;
                    }
                    else if (info.FieldType == typeof(float))
                    {
                        info.SetValue(dataObj, BitConverter.ToSingle(bytes, index));
                        index += GameConst.FLOAT_SIZE;
                    }
                    else if (info.FieldType == typeof(bool))
                    {
                        info.SetValue(dataObj, BitConverter.ToBoolean(bytes, index));
                        index += GameConst.BOOL_SIZE;
                    }
                    else if (info.FieldType == typeof(string))
                    {
                        int strLength = BitConverter.ToInt32(bytes, index);
                        index += GameConst.INT_SIZE;
                        info.SetValue(dataObj, Encoding.UTF8.GetString(bytes, index, strLength));
                        index += strLength;
                    }
                }
            
                //添加数据结构类到容器类
                object dicObj = containerType.GetField("dataDic").GetValue(containerObj);
                MethodInfo mInfo = dicObj.GetType().GetMethod("Add");
                object keyValue = classType.GetField(keyName).GetValue(dataObj);
                mInfo.Invoke(dicObj, new[] { keyValue, dataObj });
            }
        
            //记录读取的数据
            _dataDic.Add(containerType.Name, containerObj);
        }

        /// <summary>
        /// 初始化Excel二进制数据到容器中
        /// </summary>
        private void InitTableData()
        {
            _dataDic.Clear();
            if (!Directory.Exists(GameConst.DATA_BINARY_PATH))
                Directory.CreateDirectory(GameConst.DATA_BINARY_PATH);
            FileInfo[] fileInfos = new DirectoryInfo(GameConst.DATA_BINARY_PATH).GetFiles();
            foreach (FileInfo fileInfo in fileInfos)
            {
                if(fileInfo.Extension != ".zxy")
                    continue;
                var className = fileInfo.Name.Split('.')[0];
                var containerName = $"{className}Container";
                LoadTable(containerName, className);
            }
        }

        /// <summary>
        /// 获取一张表信息
        /// </summary>
        /// <typeparam name="T">容器名</typeparam>
        /// <returns></returns>
        public T GetTable<T>() where T : class
        {
            string tableName = typeof(T).Name;
            if (_dataDic.ContainsKey(tableName))
                return _dataDic[tableName] as T;
            return null;
        }

        public V GetInfo<T, K, V>(K index) where T : BaseContainer
        {
            Dictionary<K,V> dic = GetTable<T>().GetDic() as Dictionary<K, V>;
            
            if (dic == null) return default;
            if (dic.TryGetValue(index, out V val))
                return val;
            return default;
        }
    }
}