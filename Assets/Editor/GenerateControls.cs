using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Hali_Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Editor
{
    public static class GenerateControls
    {
        private class PrefabInfo
        {
            public string name;
            public Dictionary<string, Type> members = new Dictionary<string, Type>();
            public PrefabInfo(string name) => this.name = name;
        } 
        
        private const string CONTROL_PATH = "Resources/UI/Controls";
        private const string GENERATE_PATH = "Scripts/Generate/UIControls";
        private const string KEY = "_";
        
        [MenuItem("Tools/生成控件类")]
        public static void Execute()
        {
            Debug.Log($"开始生成控件类。。。");
            Stopwatch sw = Stopwatch.StartNew();
            var infos = GetAllPrefabInfo();
            var dir = Directory.CreateDirectory($"{Application.dataPath}/{GENERATE_PATH}");
            //先删除旧的
            foreach (var file in dir.GetFiles())
                file.Delete();
            foreach (var info in infos)
                GenerateControlClass(info);
            AssetDatabase.Refresh();
            Debug.Log($"生成控件类结束，耗时 {sw.ElapsedMilliseconds} ms。。。");
            sw.Stop();
        }

        private static void GenerateControlClass(PrefabInfo info)
        {
            StringBuilder content = new StringBuilder();
            content.Append("using Hali_Framework;\n");
            content.Append("using UnityEngine.UI;\n");
            content.Append("namespace Game.UI.Controls\n");
            content.Append("{\n");
            content.Append($"\tpublic partial class UI_{info.name} : ControlBase\n");
            content.Append("\t{\n");
            foreach (var member in info.members)
            {
                content.Append($"\t\tprivate {member.Value.Name} {member.Key};\n");
            }
            content.Append("\n");
            content.Append("\t\tprotected override void BindControls()\n");
            content.Append("\t\t{\n");
            content.Append("\t\t\tbase.BindControls();\n");
            foreach (var member in info.members)
            {
                content.Append($"\t\t\t{member.Key} = GetControl<{member.Value.Name}>(\"{member.Key}\");\n");
            }
            content.Append("\t\t}\n");
            content.Append("\t}\n");
            content.Append("}");
            
            File.WriteAllText($"{Application.dataPath}/{GENERATE_PATH}/UI_{info.name}.cs", content.ToString());
        }
        
        private static List<PrefabInfo> GetAllPrefabInfo()
        {
            List<PrefabInfo> prefabInfos = new List<PrefabInfo>();
            string[] files = Directory.GetFiles($"{Application.dataPath}/{CONTROL_PATH}", "*.prefab");
            for (int i = 0; i < files.Length; i++)
            {
                //去除Asset前的路径
                int index = files[i].IndexOf("Asset", StringComparison.Ordinal);
                files[i] = files[i].Substring(index, files[i].Length - index);
                //创建info并添加
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(files[i]);
                var info = new PrefabInfo(prefab.name);
                info.members = new Dictionary<string, Type>();
                DeepFindMember(prefab.transform, ref info.members);
                prefabInfos.Add(info);
            }
            return prefabInfos;
        }

        private static void DeepFindMember(Transform parent, ref Dictionary<string, Type> members)
        {
            int count = parent.childCount;
            if(count <= 0) return;
            for (int i = 0; i < count; i++)
            {
                var child = parent.GetChild(i);
                if (!child.name.Contains(KEY))//只加入含指定关键字成员
                {
                    DeepFindMember(child, ref members);
                    continue;
                }
                
                bool stopRecursion = false;
                //加入成员
                //规则：挂载复数组件的成员，只能拥有一个组件的成员，获取if优先级最高的组件
                //优先级：自定义组件 -> 复杂组件 -> 基础组件
                //自定义组件
                if (child.TryGetComponent<ControlBase>(out var cb))
                {
                    members.Add(child.name, cb.GetType());
                    stopRecursion = true;
                }
                //复杂组件
                else if(child.TryGetComponent<ScrollRect>(out var sr))
                    members.Add(child.name, typeof(ScrollRect));
                else if(child.TryGetComponent<Slider>(out var sld))
                    members.Add(child.name, typeof(Slider));
                else if(child.TryGetComponent<Toggle>(out var tog))
                    members.Add(child.name, typeof(Toggle));
                else if(child.TryGetComponent<Button>(out var btn))
                    members.Add(child.name, typeof(Button));
                else if(child.TryGetComponent<InputField>(out var ifd))
                    members.Add(child.name, typeof(InputField));
                //基础组件
                else if (child.TryGetComponent<Text>(out var txt))
                    members.Add(child.name, typeof(Text));
                else if(child.TryGetComponent<Image>(out var img))
                    members.Add(child.name, typeof(Image));
                else if(child.TryGetComponent<RawImage>(out var rawImg))
                    members.Add(child.name, typeof(RawImage));
                
                if(!stopRecursion)
                    DeepFindMember(child, ref members);
            }
        }
    }
}
