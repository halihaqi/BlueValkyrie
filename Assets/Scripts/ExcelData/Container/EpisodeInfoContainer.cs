using System.Collections.Generic;
public class EpisodeInfoContainer : BaseContainer
{
   public Dictionary<int, EpisodeInfo> dataDic = new Dictionary<int, EpisodeInfo>();
    public override object GetDic() => dataDic;
}