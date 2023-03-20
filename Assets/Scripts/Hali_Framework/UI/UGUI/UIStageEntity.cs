using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Hali_Framework
{
    public class UIStageEntity : MonoBehaviour
    {
        [SerializeField] private Transform container;
        [SerializeField] private Camera stageCamera;
        [SerializeField] private Transform pool;

        private Dictionary<string, GameObject> _poolObjs;
        private Dictionary<string, GameObject> _showObjs;
        private List<string> _recycleList;
        private List<string> _removeList;
        private bool _isClearing = false;
        private RenderTexture _rt;

        public RenderTexture RT => _rt;

        public Camera StageCamera => stageCamera;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            _poolObjs = new Dictionary<string, GameObject>();
            _showObjs = new Dictionary<string, GameObject>();
            _recycleList = new List<string>();
            _removeList = new List<string>();
            stageCamera.gameObject.SetActive(false);
        }

        private void Update()
        {
            for (int i = 0; i < _recycleList.Count; i++)
            {
                var path = _recycleList[i];
                if(_showObjs[path] == null) continue;
                PushObj(path);
                _removeList.Add(path);
            }

            for (int i = 0; i < _removeList.Count; i++)
                _recycleList.Remove(_removeList[i]);
            _removeList.Clear();
        }


        public void ShowObj(string path, UnityAction<GameObject> callback)
        {
            if(_isClearing) return;
            
            //如果还未回收但是进入回收队列，移出回收队列
            if (_recycleList.Contains(path))
                _recycleList.Remove(path);
            
            stageCamera.gameObject.SetActive(true);
            //如果show完了又show，相当于更新
            //如果没加载完又show，直接返回
            if (_showObjs.ContainsKey(path))
            {
                if(_showObjs[path] != null)
                    callback?.Invoke(_showObjs[path]);
                return;
            }
            PopObj(path, callback);
        }

        public void RecycleObj(string path)
        {
            if(_isClearing) return;
            if(_showObjs.ContainsKey(path) && !_recycleList.Contains(path))
                _recycleList.Add(path);
        }

        public void RecycleAll()
        {
            if(_isClearing) return;
            foreach (var path in _showObjs.Keys)
            {
                if(!_recycleList.Contains(path))
                    _recycleList.Add(path);
            }
        }

        public void Clear()
        {
            if(_isClearing) return;
            StartCoroutine(ClearCoroutine());
        }

        public RenderTexture BindRT(int width, int height, int depth)
        {
            if (_rt != null && _rt.width == width && _rt.height == height)
                return _rt;

            _rt = new RenderTexture(width, height, depth);
            stageCamera.targetTexture = _rt;
            return _rt;
        }

        public void SetCameraSize(float size)
        {
            stageCamera.orthographicSize = size;
        }

        private void PopObj(string path, UnityAction<GameObject> callback)
        {
            if (_poolObjs.ContainsKey(path))
            {
                var obj = _poolObjs[path];
                _poolObjs.Remove(path);
                _showObjs.Add(path, obj);
                obj.transform.SetParent(container, false);
                obj.SetActive(true);
                callback?.Invoke(obj);
                return;
            }
            
            _showObjs.Add(path, null);
            ResMgr.Instance.LoadAsync<GameObject>(path, go =>
            {
                _showObjs[path] = go;
                go.SetActive(true);
                go.transform.SetParent(container, false);
                callback?.Invoke(go);
            });
        }

        private void PushObj(string path)
        {
            var obj = _showObjs[path];
            _showObjs.Remove(path);
            obj.SetActive(false);
            obj.transform.SetParent(pool, false);
            _poolObjs.Add(path, obj);
        }

        private IEnumerator ClearCoroutine()
        {
            //先全部入池
            RecycleAll();
            _isClearing = true;
            stageCamera.gameObject.SetActive(false);
            while (_recycleList.Count > 0)
            {
                yield return null;
            }
            
            //再将池清空
            foreach (var poolObj in _poolObjs.Values)
            {
                Destroy(poolObj);
            }
            _poolObjs.Clear();
            _showObjs.Clear();
            _recycleList.Clear();
            _isClearing = false;
        }
    }
}