using System.Collections.Generic;
public class ShopItemInfoContainer : BaseContainer
{
   public Dictionary<int, ShopItemInfo> dataDic = new Dictionary<int, ShopItemInfo>();
    public override object GetDic() => dataDic;
}