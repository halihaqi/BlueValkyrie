using System.Collections.Generic;
public class ExpInfoContainer : BaseContainer
{
   public Dictionary<int, ExpInfo> dataDic = new Dictionary<int, ExpInfo>();
    public override object GetDic() => dataDic;
}