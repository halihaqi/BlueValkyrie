using DG.Tweening;
using Game.BattleScene;
using Game.BattleScene.BattleRole;
using Game.Utils;
using Hali_Framework;
using UnityEngine;

namespace Game.UI.Controls
{
    public partial class UI_battle_role_form : ControlBase
    {
        [SerializeField] private RectTransform btnGroup;
        private Vector2 _oriBtnPos;
        private IBattleRole _role;
        
        protected internal override void OnInit()
        {
            base.OnInit();
            btn_fight.onClick.AddListener(OnFight);
            btn_rest.onClick.AddListener(OnRest);
            _oriBtnPos = btnGroup.anchoredPosition;
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            btn_fight.onClick.RemoveListener(OnFight);
            btn_rest.onClick.RemoveListener(OnRest);
        }

        public void SetData(IBattleRole role, bool canChoose)
        {
            _role = role;
            ResMgr.Instance.LoadAsync<Sprite>(GameConst.RES_GROUP_UI, 
                ResPath.GetRoleIcon(role.RoleType, role.RoleName), img =>
            {
                img_head.sprite = img;
            });
            ResMgr.Instance.LoadAsync<Sprite>(GameConst.RES_GROUP_UI, 
                ResPath.GetGunIcon(role.AtkType), img =>
                {
                    img_atkType.sprite = img;
                });
            ResMgr.Instance.LoadAsync<Sprite>(GameConst.RES_GROUP_UI, 
                ResPath.GetAmmonIcon(role.RoleType), img =>
                {
                    img_ammo.sprite = img;
                });
            img_top.color = ResPath.GetRoleColor(role.RoleType);
            sld_hp.maxValue = role.RoleState.maxHp;
            sld_hp.value = role.CurHp;
            sld_ap.maxValue = role.RoleState.maxAp;
            sld_ap.value = role.CurAp;
            txt_type.text = role.RoleType.ToString();
            txt_name.text = role.RoleName;
            txt_hp.text = $"{role.CurHp}/{role.RoleState.maxHp}";
            txt_ammo.text = $"{role.CurAmmo}/{role.RoleState.maxAmmo}";
            btnGroup.anchoredPosition = canChoose ? _oriBtnPos : new Vector2(_oriBtnPos.x, _oriBtnPos.y + 100);
        }

        private void OnFight()
        {
            var fsm = FsmMgr.Instance.GetFsm<BattleMaster>(BattleConst.BATTLE_FSM);
            fsm.Owner.WarRoleFight(_role);
        }

        private void OnRest()
        {
            var fsm = FsmMgr.Instance.GetFsm<BattleMaster>(BattleConst.BATTLE_FSM);
            fsm.Owner.WarRoleReset();
        }
    }
}