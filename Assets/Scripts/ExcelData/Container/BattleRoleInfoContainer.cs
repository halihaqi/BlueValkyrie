using System.Collections.Generic;
public class BattleRoleInfoContainer : BaseContainer
{
   public Dictionary<int, BattleRoleInfo> dataDic = new Dictionary<int, BattleRoleInfo>();
    public override object GetDic() => dataDic;
}