using System.Collections.Generic;
public class EquipExpInfoContainer : BaseContainer
{
   public Dictionary<int, EquipExpInfo> dataDic = new Dictionary<int, EquipExpInfo>();
    public override object GetDic() => dataDic;
}