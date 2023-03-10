using System.Collections.Generic;
public class RoleLevelInfoContainer : BaseContainer
{
   public Dictionary<int, RoleLevelInfo> dataDic = new Dictionary<int, RoleLevelInfo>();
    public override object GetDic() => dataDic;
}