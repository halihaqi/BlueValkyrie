using System.Collections.Generic;
public class ShopInfoContainer : BaseContainer
{
   public Dictionary<int, ShopInfo> dataDic = new Dictionary<int, ShopInfo>();
    public override object GetDic() => dataDic;
}