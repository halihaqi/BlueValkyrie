using System.Collections.Generic;
public class ItemInfoContainer : BaseContainer
{
   public Dictionary<int, ItemInfo> dataDic = new Dictionary<int, ItemInfo>();
    public override object GetDic() => dataDic;
}