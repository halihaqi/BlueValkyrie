using System.Collections.Generic;
public class ShopTypeInfoContainer : BaseContainer
{
   public Dictionary<int, ShopTypeInfo> dataDic = new Dictionary<int, ShopTypeInfo>();
    public override object GetDic() => dataDic;
}