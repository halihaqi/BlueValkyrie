using Game.BattleScene;
using Game.Entity;
using Game.Managers;
using Game.Utils;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Controls.Battle
{
    public partial class UI_battle_enemy_form : ControlBase
    {
        private Image _imgHead;
        private Slider _sldHp;
        private Slider _sldAp;
        private Text _txtName;
        private Text _txtHp;

        private BattleEnemyEntity _enemy;

        protected internal override void OnInit()
        {
            base.OnInit();
            _imgHead = GetControl<Image>("img_head");
            _sldHp = GetControl<Slider>("sld_hp");
            _sldAp = GetControl<Slider>("sld_ap");
            _txtName = GetControl<Text>("txt_name");
            _txtHp = GetControl<Text>("txt_hp");
        }

        public void SetData(int roleIndex)
        {
            var bm = FsmMgr.Instance.GetFsm<BattleMaster>(BattleConst.BATTLE_FSM).Owner;
            _enemy = bm.GetEnemyEntity(roleIndex);
            
            ResMgr.Instance.LoadAsync<Sprite>(GameConst.RES_GROUP_UI, ResPath.GetEnemyIcon(_enemy.EnemyInfo.roleName),
                spr =>
                {
                    _imgHead.sprite = spr;
                });

            _txtName.text = _enemy.EnemyInfo.roleName;
            _sldHp.maxValue = _enemy.MaxHp;
            _sldHp.value = _enemy.CurHp;
            _sldAp.maxValue = _enemy.MaxAp;
            _sldAp.value = _enemy.CurAp;
            _txtHp.text = $"{_enemy.CurHp}/{_enemy.MaxHp}";
        }
    }
}