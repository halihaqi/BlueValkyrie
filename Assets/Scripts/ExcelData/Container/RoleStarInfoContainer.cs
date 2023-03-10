using System.Collections.Generic;
public class RoleStarInfoContainer : BaseContainer
{
   public Dictionary<int, RoleStarInfo> dataDic = new Dictionary<int, RoleStarInfo>();
    public override object GetDic() => dataDic;
}