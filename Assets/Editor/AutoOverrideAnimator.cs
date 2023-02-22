using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hali_Framework;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    //todo [CustomEditor(typeof(AnimatorOverrideController))]
    public class AutoOverrideAnimator : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            AnimatorOverrideController anim = (AnimatorOverrideController)target;

            if (GUILayout.Button("Auto Override"))
            {
                //获取clip路径
                string objPath = AssetDatabase.GetAssetPath(target);
                string dirPath = objPath;
                for (int i = dirPath.Length - 1; i >= 0; i--)
                {
                    if (dirPath[i] == '/')
                    {
                        dirPath = dirPath.Remove(i + 1);
                        break;
                    }
                }
                FileInfo[] infos = new FileInfo(objPath).Directory.GetFiles();
                var clipPaths = from f in infos where f.Extension.Equals(".anim") select dirPath + f.Name;
                
                //切割clip路径
                List<string> splitPaths = new List<string>();
                foreach (var clipPath in clipPaths)
                {
                    var strs = clipPath.Split('_');
                    splitPaths.Add(strs[^2] + "_" + strs[^1]);
                }

                //设置重载Animator
                var overrideControllerProperty = serializedObject.FindProperty("m_Controller");
                var oriAnimator = ResMgr.Instance.Load<RuntimeAnimatorController>("Animator/Students/Gehenna/Aru/Aru_anim");
                overrideControllerProperty.objectReferenceValue = oriAnimator;
                
                //设置重载动画
                var clipArrayProperty = serializedObject.FindProperty("m_Clips");
                clipArrayProperty.arraySize = oriAnimator.animationClips.Length;
                for (int i = 0; i < clipArrayProperty.arraySize; i++)
                {
                    var clipProperty = clipArrayProperty.GetArrayElementAtIndex(i);
                    var oriClipProperty = clipProperty.FindPropertyRelative("originalClip");
                    var newClipProperty = clipProperty.FindPropertyRelative("overrideClip");
                    //设置源动画
                    oriClipProperty.objectReferenceValue = oriAnimator.animationClips[i];
                    //设置重载动画
                    var strs = oriAnimator.animationClips[i].name.Split('_');
                }
                
                //保存修改
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
