using System;
using System.Collections.Generic;
using Game.Model;
using UnityEngine;

namespace Game.Entity
{
    [Serializable]
    public class CampStation
    {
        public RoleType type;
        public List<FlagEntity> flags;//可能有多个己方旗帜
        public List<Transform> roles;
    }
    public class EpisodeEntity : MonoBehaviour
    {
        public List<CampStation> campPos;
        public List<Transform> shelterPos;
    }
}