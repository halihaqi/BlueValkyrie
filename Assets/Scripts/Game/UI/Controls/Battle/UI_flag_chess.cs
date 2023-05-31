using Game.Entity;
using Hali_Framework;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public partial class UI_flag_chess : ControlBase
    {
        public void SetData(FlagType type)
        {
            none_chess.gameObject.SetActive(type == FlagType.None);
            student_chess.gameObject.SetActive(type == FlagType.Student);
            enemy_chess.gameObject.SetActive(type == FlagType.Enemy);
        }
    }
}