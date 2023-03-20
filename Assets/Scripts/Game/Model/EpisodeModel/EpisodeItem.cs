using System;

namespace Game.Model.EpisodeModel
{
    [Serializable]
    public class EpisodeItem
    {
        public int episodeId;
        public int passCount;
        public int minPassTime;
        public int minPassRound;
        public int minPassDeadCount;
        
        public EpisodeItem(){}

        public EpisodeItem(int episodeId)
        {
            this.episodeId = episodeId;
            passCount = 0;
            minPassTime = int.MaxValue;
            minPassRound = int.MaxValue;
            minPassDeadCount = int.MaxValue;
        }
    }
}