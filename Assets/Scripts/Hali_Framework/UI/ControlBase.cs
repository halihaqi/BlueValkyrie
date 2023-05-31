using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hali_Framework
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class ControlBase : UIBehaviour
    {
        private const char KEY = '_';
        protected CanvasGroup canvasGroup;
        private Dictionary<string, List<UIBehaviour>> _controlDic;//控件名为键
        private Dictionary<Type, List<ControlBase>> _addControlDic;//自定义控件类为键

        protected internal virtual void OnInit()
        {
            _controlDic = new Dictionary<string, List<UIBehaviour>>();
            _addControlDic = new Dictionary<Type, List<ControlBase>>();
            canvasGroup = GetComponent<CanvasGroup>();
            //搜索UI组件添加到容器中
            FindChildrenControls(this.transform);
            BindControls();//用于自动生成控件的成员绑定
        }

        protected internal virtual void OnRecycle()
        {
            foreach (var controlList in _controlDic.Values)
            {
                foreach (var control in controlList)
                {
                    if(control is ControlBase cb)
                        cb.OnRecycle();
                }
            }
            RecycleAddCustomControls();
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
        /// 获得所有该种类UI控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetControls<T>() where T : UIBehaviour
        {
            List<T> list = new List<T>();
            foreach (var control in _controlDic.Values)
            {
                T item = control.Find(o => o is T) as T;
                if(item != null)
                    list.Add(item);
            }

            return list;
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
                //不加下划线的控件不加入
                if (!child.name.Contains(KEY))
                {
                    FindChildrenControls(child);
                    continue;
                }
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

        public void SetInteractable(bool isInteractable)
            => canvasGroup.interactable = isInteractable;

        public void SetBlocksRaycasts(bool enable)
        {
            canvasGroup.blocksRaycasts = enable;
        }

        public void SetAlpha(float alpha) => canvasGroup.alpha = Mathf.Clamp(alpha, 0, 1);

        protected virtual void BindControls(){}

        #region CustomControl

        public List<T> GetAddControls<T>() where T : ControlBase
        {
            if (_addControlDic.ContainsKey(typeof(T)))
                return _addControlDic[typeof(T)] as List<T>;

            return null;
        }
        
        /// <summary>
        /// 实例化自定义控件并加入控件字典，自动触发控件的OnInit方法
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        public void InitCustomControl<T>(string path, Action<T> callback) where T : ControlBase
        {
            ResMgr.Instance.LoadAsync<GameObject>(path, go =>
            {
                var control = go.GetComponent<T>();
                if (control == null)
                    throw new Exception($"{control.GetType().Name} has no {typeof(T)}.");
                AddCustomControl(control);
                
                callback?.Invoke(control);
            });
        }

        public void AddCustomControl(ControlBase cb)
        {
            Type type = cb.GetType();
            if (_addControlDic.ContainsKey(type))
            {
                _addControlDic[type] ??= new List<ControlBase>();
                _addControlDic[type].Add(cb);
            }
            else
                _addControlDic.Add(type, new List<ControlBase> { cb });
            cb.OnInit();
        }

        public bool RemoveCustomControl<T>() where T : ControlBase
            => RemoveCustomControl(typeof(T));

        public bool RemoveCustomControl(Type cbType)
        {
            if (_addControlDic.ContainsKey(cbType))
            {
                var removeCbs = _addControlDic[cbType];
                _addControlDic.Remove(cbType);
                foreach (var cb in removeCbs)
                {
                    cb.OnRecycle();
                    Destroy(cb.gameObject);
                }
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// 回收添加的自定义控件进池，OnRecycle调用
        /// </summary>
        private void RecycleAddCustomControls()
        {
            foreach (var type in _addControlDic.Keys)
            {
                if (_addControlDic.ContainsKey(type))
                {
                    foreach (var cb in _addControlDic[type])
                    {
                        cb.OnRecycle();
                        Destroy(cb.gameObject);
                    }
                }
            }
            _addControlDic.Clear();
        }

        #endregion
    }
}