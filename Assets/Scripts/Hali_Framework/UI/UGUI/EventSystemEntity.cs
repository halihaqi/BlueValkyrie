using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Hali_Framework
{
    [RequireComponent(typeof(EventSystem))]
    [RequireComponent(typeof(StandaloneInputModule))]
    public class EventSystemEntity : MonoBehaviour
    {
        private EventSystem _eventSystem;
        private StandaloneInputModule _standaloneInputModule;
        
        private void Awake()
        {
            DontDestroyOnLoad(this);
            _eventSystem = GetComponent<EventSystem>();
            _standaloneInputModule = GetComponent<StandaloneInputModule>();
            gameObject.layer = LayerMask.NameToLayer("UI");
        }
    }
}