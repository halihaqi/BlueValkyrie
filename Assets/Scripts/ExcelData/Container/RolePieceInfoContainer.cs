using System.Collections.Generic;
public class RolePieceInfoContainer : BaseContainer
{
   public Dictionary<int, RolePieceInfo> dataDic = new Dictionary<int, RolePieceInfo>();
    public override object GetDic() => dataDic;
}