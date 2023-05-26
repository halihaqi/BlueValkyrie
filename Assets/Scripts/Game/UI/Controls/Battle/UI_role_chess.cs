using Game.BattleScene.BattleRole;
using Hali_Framework;

namespace Game.UI.Controls
{
    public partial class UI_role_chess : ControlBase
    {
        private IBattleRole _role;

        public void SetData(IBattleRole role)
        {
            _role = role;
            
        }
    }
}