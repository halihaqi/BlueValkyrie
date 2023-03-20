using System.Collections.Generic;
public class EnemyInfoContainer : BaseContainer
{
   public Dictionary<int, EnemyInfo> dataDic = new Dictionary<int, EnemyInfo>();
    public override object GetDic() => dataDic;
}