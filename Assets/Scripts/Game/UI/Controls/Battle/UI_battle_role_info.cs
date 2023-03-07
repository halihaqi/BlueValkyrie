using Game.Utils;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Controls.Battle
{
    public class UI_battle_role_info : ControlBase
    {
        private Image _imgSelect;
        private Image _imgHead;
        private Text _txtName;

        private bool _selected = false;

        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                _imgSelect.gameObject.SetActive(_selected);
            }
        }
        
        protected internal override void OnInit()
        {
            base.OnInit();
            _imgSelect = GetControl<Image>("img_select");
            _imgHead = GetControl<Image>("img_head");
            _txtName = GetControl<Text>("txt_name");
        }

        public void SetData(BattleRoleInfo info)
        {
            _imgSelect.gameObject.SetActive(_selected);
            ResMgr.Instance.LoadAsync<Sprite>(GameConst.RES_GROUP_UI, ResPath.GetStudentIcon(info.id), img =>
            {
                _imgHead.sprite = img;
            });
            _txtName.text = info.name;
        }
    }
}