using System;
using Spine.Unity;
using UnityEngine;

namespace Game.Entity
{
    public class AronaEntity : MonoBehaviour
    {
        private SkeletonGraphic _sg;

        private void Awake()
        {
            _sg = GetComponent<SkeletonGraphic>();
        }
    }
}