using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Hali_Framework
{
    public class UIStageEntity : MonoBehaviour
    {
        [SerializeField] private Transform container;
        [SerializeField] private Camera uiCamera;
        [SerializeField] private Transform pool;

        private Dictionary<string, GameObject> _pool;
        private Dictionary<string, UnityAction<GameObject>> _loadingDic;
        private string _curPath;
        private GameObject _curObj;

        public GameObject CurObj => _curObj;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            _pool = new Dictionary<string, GameObject>();
            _loadingDic = new Dictionary<string, UnityAction<GameObject>>();
            uiCamera.gameObject.SetActive(false);
        }


        public void ShowObj(string path, UnityAction<GameObject> callback)
        {
            //先隐藏当前的再显示
            HideCurObj();
            uiCamera.gameObject.SetActive(true);
            PopObj(path, callback);
        }

        public void HideCurObj()
        {
            uiCamera.gameObject.SetActive(false);
            if(string.IsNullOrEmpty(_curPath)) return;
            //如果正在加载，加载完成后Push进池
            if (_curObj == null)
            {
                _loadingDic[_curPath] += obj =>
                {
                    PushObj(_curPath, obj);
                    _curObj = null;
                    _curPath = null;
                };
                return;
            }
            
            PushObj(_curPath, _curObj);
            _curObj = null;
            _curPath = null;
        }

        public void ClearObj()
        {
            HideCurObj();
            uiCamera.gameObject.SetActive(false);
            foreach (var obj in _pool.Values)
            {
                Destroy(obj);
            }
            _pool.Clear();
            foreach (var path in _loadingDic.Keys)
            {
                _loadingDic[path] += obj => Destroy(obj);
            }
            _loadingDic.Clear();
        }

        private void PopObj(string path, UnityAction<GameObject> callback)
        {
            //防止重复加载
            if(_loadingDic.ContainsKey(path)) return;

            _curObj = null;
            if (_pool.ContainsKey(path))
            {
                _curObj = _pool[path];
                _curPath = path;
                _pool.Remove(path);
                _curObj.transform.SetParent(container, false);
                _curObj.SetActive(true);
                callback?.Invoke(_curObj);
                return;
            }

            _curPath = path;
            _loadingDic.Add(path, callback);
            ResMgr.Instance.LoadAsync<GameObject>(path, obj =>
            {
                _curObj = obj;
                obj.SetActive(true);
                obj.transform.SetParent(container, false);
                _loadingDic[path]?.Invoke(obj);
                _loadingDic.Remove(path);
            });
        }

        private void PushObj(string path, GameObject obj)
        {
            if (_pool.ContainsKey(path))
            {
                Destroy(obj);
                return;
            }
            obj.SetActive(false);
            obj.transform.SetParent(pool, false);
            _pool.Add(path, obj);
        }
    }
}