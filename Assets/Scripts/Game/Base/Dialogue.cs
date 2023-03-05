using System;
using System.Collections;
using System.Collections.Generic;
using Hali_Framework;
using UnityEngine;

namespace Game.Managers
{
    [Serializable]
    public class Dialogue
    {
        private const char KEY = '\n';
        public string[] text;//对话文本
        public float interval;//间隔时间
        /// <summary>
        /// 对话中插入的事件列表，key对应第几句
        /// </summary>
        public readonly Dictionary<int, Action<string>> callbacks;

        public Dialogue(TextAsset asset, float interval)
        {
            text = asset.text.Split(KEY);
            this.interval = interval;
            callbacks = new Dictionary<int, Action<string>>(text.Length);
            for (int i = 0; i < text.Length; i++)
                callbacks.Add(i, null);
        }

        public void AddSentenceCallback(int index, Action<string> callback)
        {
            if (callbacks.ContainsKey(index))
            {
                callbacks[index] -= callback;
                callbacks[index] += callback;
            }
        }
        
        public void RemoveSentenceCallback(int index, Action<string> callback)
        {
            if (callbacks.ContainsKey(index))
                callbacks[index] -= callback;
        }

        public void AddLastSentenceCallback(Action<string> callback) =>
            AddSentenceCallback(callbacks.Count - 1, callback);

        public void RemoveSentenceCallback(Action<string> callback) =>
            RemoveSentenceCallback(callbacks.Count - 1, callback);
    }
}