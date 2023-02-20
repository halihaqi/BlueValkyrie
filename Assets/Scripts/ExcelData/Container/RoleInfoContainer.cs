using System.Collections.Generic;
public class RoleInfoContainer : BaseContainer
{
   public Dictionary<int, RoleInfo> dataDic = new Dictionary<int, RoleInfo>();
    public override object GetDic() => dataDic;
}