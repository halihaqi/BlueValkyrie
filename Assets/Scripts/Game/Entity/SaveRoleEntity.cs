using Game.Base;
using Game.UI.Game;
using Hali_Framework;

namespace Game.Entity
{
    public class SaveRoleEntity : NPCBase
    {
        protected override void OnDialogueOver()
        {
            base.OnDialogueOver();
            UIMgr.Instance.ShowPanel<SaveLoadPanel>();
        }
    }
}