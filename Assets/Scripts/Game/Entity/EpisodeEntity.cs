using System.Collections.Generic;
using UnityEngine;

namespace Game.Entity
{
    public class EpisodeEntity : MonoBehaviour
    {
        public List<Transform> studentPos;
        public List<Transform> enemyPos;
        public List<Transform> shelterPos;
        public List<FlagEntity> flagPos;
    }
}