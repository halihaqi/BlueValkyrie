using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Hali_Framework
{
    public class EventSystemEntity : MonoBehaviour
    {
        private EventSystem _eventSystem;
        private StandaloneInputModule _standaloneInputModule;
        
        private void Awake()
        {
            DontDestroyOnLoad(this);
            if (!gameObject.TryGetComponent(out _eventSystem))
                _eventSystem = gameObject.AddComponent<EventSystem>();
            if (!gameObject.TryGetComponent(out _standaloneInputModule))
                _standaloneInputModule = gameObject.AddComponent<StandaloneInputModule>();
        }
    }
}