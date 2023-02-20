using System.Collections.Generic;
public class ChooseRoleInfoContainer : BaseContainer
{
   public Dictionary<int, ChooseRoleInfo> dataDic = new Dictionary<int, ChooseRoleInfo>();
    public override object GetDic() => dataDic;
}