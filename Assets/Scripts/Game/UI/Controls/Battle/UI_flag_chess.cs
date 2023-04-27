using Game.Entity;
using Hali_Framework;
using UnityEngine.UI;

namespace Game.UI.Controls.Battle
{
    public partial class UI_flag_chess : ControlBase
    {
        private Image _noneChess;
        private Image _studentChess;
        private Image _enemyChess;

        protected internal override void OnInit()
        {
            base.OnInit();
            _noneChess = GetControl<Image>("none_chess");
            _studentChess = GetControl<Image>("student_chess");
            _enemyChess = GetControl<Image>("enemy_chess");
        }

        public void SetData(FlagType type)
        {
            _noneChess.gameObject.SetActive(type == FlagType.None);
            _studentChess.gameObject.SetActive(type == FlagType.Student);
            _enemyChess.gameObject.SetActive(type == FlagType.Enemy);
        }
    }
}