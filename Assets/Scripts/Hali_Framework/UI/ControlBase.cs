using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hali_Framework
{
    public abstract class ControlBase : UIBehaviour
    {
        private Dictionary<string, List<UIBehaviour>> _controlDic;

        protected internal virtual void OnInit()
        {
            _controlDic = new Dictionary<string, List<UIBehaviour>>();
            //搜索UI组件添加到容器中
            FindChildrenControls(this.transform);
        }

        /// <summary>  
        /// 获得物体挂载的UI组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="controlName">物体名</param>
        /// <returns></returns>
        public T GetControl<T>(string controlName) where T : UIBehaviour
        {
            //每个物体只会挂载一个同种类的组件，所以不会重复
            if (_controlDic.ContainsKey(controlName))
            {
                for (int i = 0; i < _controlDic[controlName].Count; i++)
                {
                    if (_controlDic[controlName][i] is T control)
                        return control;
                }
            }
            Debug.Log($"No UIControl named:{controlName}");
            return null;
        }

        /// <summary>
        /// 搜索所有子物体的UI组件并添加进字典容器中，
        /// 如果是ControlBase下的子物体将不会添加，由ControlBase管理
        /// </summary>
        private void FindChildrenControls(Transform parent)
        {
            if(parent.childCount <=0) return;

            //搜索所有子物体的组件
            Transform child;
            bool stopRecursion = false;
            for (int i = 0; i < parent.childCount; i++)
            {
                stopRecursion = false;
                child = parent.GetChild(i);
                var controls = child.GetComponents<UIBehaviour>();
                foreach (var control in controls)
                {
                    if (_controlDic.ContainsKey(control.name))
                        _controlDic[control.name].Add(control);
                    else
                        _controlDic.Add(control.name, new List<UIBehaviour> { control });
                        
                    //如果是ControlBase，中断搜索，由ControlBase管理子控件
                    if (control is ControlBase cb)
                    {
                        cb.OnInit();
                        stopRecursion = true;
                    }
                }

                //递归搜索所有子物体
                if(!stopRecursion)
                    FindChildrenControls(child);
            }
        }
    }
}