using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Hali_Framework
{
    public class AudioMgr : Singleton<AudioMgr>, IModule
    {
        //背景音乐
        private AudioSource _bkMusic = null;
        private float _bkMusicVolume = 1;
        private bool _bkMusicOn = true;

        //音效
        private GameObject _soundObj = null;
        private Dictionary<string, AudioSource> _soundDic;
        private float _soundVolume = 1;
        private bool _soundOn = true;

        int IModule.Priority => 1;

        void IModule.Init()
        {
            _soundDic = new Dictionary<string, AudioSource>();
            MonoMgr.Instance.AddUpdateListener(MusicUpdate);
        }

        void IModule.Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        void IModule.Dispose()
        {
            _soundDic.Clear();
            MonoMgr.Instance.RemoveUpdateListener(MusicUpdate);
        }

        /// <summary>
        /// 每帧判断音效是否播放完毕
        /// </summary>
        private void MusicUpdate()
        {
            foreach (string key in _soundDic.Keys)
            {
                if (!_soundDic[key].isPlaying)
                {
                    GameObject.Destroy(_soundDic[key]);
                    _soundDic.Remove(key);
                }
            }
        }


        #region 背景音乐公共方法
        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="path">音乐路径</param>
        public void PlayBkMusic(string path)
        {
            //如果为空，在场景中新建并添加AudioSource
            if (_bkMusic == null)
            {
                GameObject obj = new GameObject("BkMusic");
                _bkMusic = obj.AddComponent<AudioSource>();
            }

            ResMgr.Instance.LoadAsync<AudioClip>(path, (clip) =>
            {
                _bkMusic.clip = clip;
                _bkMusic.loop = true;
                _bkMusic.mute = !_bkMusicOn;
                _bkMusic.volume = _bkMusicVolume;
                _bkMusic.Play();
            });
        }

        /// <summary>
        /// 改变背景音乐大小
        /// </summary>
        /// <param name="volume">音量</param>
        public void ChangeBkMusicVolume(float volume)
        {
            _bkMusicVolume = volume;
            if (_bkMusic != null)
                _bkMusic.volume = _bkMusicVolume;
        }

        /// <summary>
        /// 改变背景音乐开关
        /// </summary>
        /// <param name="isOn">是否开启</param>
        public void ChangeBkMusicOn(bool isOn)
        {
            _bkMusicOn = isOn;
            if(_bkMusic != null)
                _bkMusic.mute = !_bkMusicOn;
        }

        /// <summary>
        /// 暂停背景音乐
        /// </summary>
        public void PauseBkMusic()
        {
            if (_bkMusic == null)
            {
                Debug.Log("No BkMusic in Scene");
                return;
            }
            _bkMusic.Pause();
        }

        /// <summary>
        /// 停止背景音乐
        /// </summary>
        public void StopBkMusic()
        {
            if (_bkMusic == null)
            {
                Debug.Log("No BkMusic in Scene");
                return;
            }
            _bkMusic.Stop();
        }
        #endregion

        #region 音效公共方法
        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="path">音效路径</param>
        /// <param name="callback">回调函数,获得source</param>
        public void PlaySound(string path, bool isLoop, UnityAction<AudioSource> callback = null)
        {
            //如果场景中没有就新建Sound空物体
            if(_soundObj == null)
                _soundObj = new GameObject("Sound");
            //异步加载音效，加载完成后播放并加入soundList
            ResMgr.Instance.LoadAsync<AudioClip>(path, (clip) =>
            {
                AudioSource source = _soundObj.AddComponent<AudioSource>();
                source.clip = clip;
                source.volume = _soundVolume;
                source.mute = !_soundOn;
                source.loop = isLoop;
                source.Play();
                _soundDic.Add(path, source);
                callback?.Invoke(source);
            });
        }

        /// <summary>
        /// 改变单个音效大小
        /// </summary>
        /// <param name="path">音效路径</param>
        /// <param name="volume">音量</param>
        public void ChangeSoundVolume(string path, float volume)
        {
            if (_soundDic.ContainsKey(path))
                _soundDic[path].volume = volume;
        }

        /// <summary>
        /// 改变所有音效大小
        /// </summary>
        /// <param name="volume">音量</param>
        public void ChangeAllSoundVolume(float volume)
        {
            _soundVolume = volume;
            foreach (string key in _soundDic.Keys)
            {
                _soundDic[key].volume = volume;
            }
        }

        /// <summary>
        /// 改变单个音效开关
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="isOn">是否开启</param>
        public void ChangeSoundOn(string path, bool isOn)
        {
            if (_soundDic.ContainsKey(path))
                _soundDic[path].mute = !isOn;
        }

        /// <summary>
        /// 改变所有音效开关
        /// </summary>
        /// <param name="isOn">是否开启</param>
        public void ChangeAllSoundOn(bool isOn)
        {
            _soundOn = isOn;
            foreach (string key in _soundDic.Keys)
            {
                _soundDic[key].mute = !isOn;
            }
        }

        /// <summary>
        /// 停止音效
        /// </summary>
        /// <param name="source">音效组件</param>
        public void StopSound(string path)
        {
            if (!_soundDic.ContainsKey(path))
            {
                Debug.Log("No Sound in Scene");
                return;
            }
            _soundDic[path].Stop();
            Object.Destroy(_soundDic[path]);
            _soundDic.Remove(path);
        }

        /// <summary>
        /// 停止所有音效
        /// </summary>
        public void StopAllSound()
        {
            foreach (string key in _soundDic.Keys)
            {
                _soundDic[key].Stop();
                Object.Destroy(_soundDic[key]);
            }
            _soundDic.Clear();
        }

        #endregion
    }
}
