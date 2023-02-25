using System.Collections.Generic;
public class EquipInfoContainer : BaseContainer
{
   public Dictionary<int, EquipInfo> dataDic = new Dictionary<int, EquipInfo>();
    public override object GetDic() => dataDic;
}