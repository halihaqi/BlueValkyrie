using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Hali_Framework
{
    public class SceneMgr : Singleton<SceneMgr>, IModule
    {
        public int Priority => 0;
        void IModule.Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        void IModule.Dispose()
        {
        }
        
        /// <summary>
        /// 加载场景(同步)
        /// </summary>
        /// <param name="sceneName">场景名</param>
        public void LoadScene(string sceneName)
        {
            //切换下一个场景前，清空对象池和资源字典，释放空间
            ResMgr.Instance.ClearAllRes();
            ObjectPoolMgr.Instance.Clear();
            SceneManager.LoadScene(sceneName);
            EventMgr.Instance.Clear();
        }

        /// <summary>
        /// 加载场景(异步)
        /// 加载过程中会分发事件"Loading"
        /// 加载结束会分发无参事件"LoadComplete"
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="callback">完成回调函数</param>
        public void LoadSceneAsync(string sceneName, UnityAction callback)
        {
            ResMgr.Instance.ClearAllRes();
            ObjectPoolMgr.Instance.Clear();
            MonoMgr.Instance.StartCoroutine(AsyncLoad(sceneName, callback));
        }

        /// <summary>
        /// 加载场景(显示LoadingUI界面)
        /// 加载过程中会分发有参事件"Loading",参数为加载进度
        /// 加载结束会分发无参事件"LoadComplete"，会自动隐藏加载面板
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="callback">加载完成的回调</param>
        public void LoadSceneWithPanel<T>(string sceneName, UnityAction callback) where T : PanelBase
        {
            //先显示LoadingUI
            UIMgr.Instance.ShowPanel<T>(GameConst.UIGROUP_SYS, callback: panel =>
            {
                //显示完后开始切换场景
                ResMgr.Instance.ClearAllRes();
                ObjectPoolMgr.Instance.Clear();
                MonoMgr.Instance.StartCoroutine(AsyncLoadWithPanel(sceneName, panel, callback));
            });
        }

        public bool IsCurScene(string sceneName) => SceneManager.GetActiveScene().name == sceneName;

        //异步加载场景协程
        IEnumerator AsyncLoad(string name,UnityAction action)
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync(name);
            while (!ao.isDone)
            {
                yield return ao.progress;
            }
            //等一帧，为界面更新提供时机
            yield return null;
            EventMgr.Instance.TriggerEvent(ClientEvent.LOAD_COMPLETE);
            action?.Invoke();
        }

        //异步加载场景协程(有UI遮挡)
        IEnumerator AsyncLoadWithPanel(string name, PanelBase panel, UnityAction callback)
        {
            //申明toProgress表示假的加载进度
            //因为ao.progress在加载小场景时变化太快，效果不好
            int toProgress = 0;
            AsyncOperation ao = SceneManager.LoadSceneAsync(name);
            ao.allowSceneActivation = false;

            //在allowSceneActivation = true之前progress只会加载到0.9
            //需要手动设置
            while (ao.progress < 0.9f)
            {
                //这里表示的假的进度变化，如果小于ao.progress，每帧加1
                while (toProgress < (int)(ao.progress * 100))
                {
                    toProgress++;
                    EventMgr.Instance.TriggerEvent(ClientEvent.LOADING, toProgress);
                    yield return toProgress;
                }
            }
            //加载到0.9后手动设置为true
            //会继续加载0.9~1的资源并开启场景
            //此时还不应该显示场景
            ao.allowSceneActivation = true;
            //isDone只有在allowSceneActivation为true并且加载完后才为true
            //可以通过这个判断是否加载完成
            while (!ao.isDone)
            {
                toProgress = 95;
                EventMgr.Instance.TriggerEvent(ClientEvent.LOADING, toProgress);
                yield return toProgress;
            }
            toProgress = 100;
            EventMgr.Instance.TriggerEvent(ClientEvent.LOADING, toProgress);
            //等一帧，为界面更新提供时机
            yield return null;
            callback?.Invoke();
            UIMgr.Instance.HidePanel(panel);
            EventMgr.Instance.TriggerEvent(ClientEvent.LOAD_COMPLETE);
        }
    }
}
