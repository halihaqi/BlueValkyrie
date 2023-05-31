using Game.BattleScene;
using Game.Entity;
using Game.Managers;
using Game.Utils;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Battle
{
    public class BattlePanel : PanelBase
    {
        // public float sizeDelta = 0.03f;
        //
        // private UI_move_group _moveGroup;
        // private UI_shoot_group _shootGroup;
        // private Image _imgHead;
        // private Image _imgHeadEnemy;
        // private Text _txtName;
        // private Slider _sldHp;
        // private Slider _sldTargetHp;
        // private Text _txtHp;
        // private Text _txtRound;
        //
        // private BattleRoleEntity _battleRole;
        // private RoleInfo _roleInfo;
        // private BattleMaster _bm;
        // private BattleRoleEntity _target;
        //
        // public bool IsAim
        // {
        //     get => !_moveGroup.gameObject.activeSelf && _shootGroup.gameObject.activeSelf;
        //     set
        //     {
        //         _moveGroup.SetActive(!value);
        //         _shootGroup.SetActive(value);
        //     }
        // }
        //
        // protected internal override void OnInit(object userData)
        // {
        //     base.OnInit(userData);
        //     _moveGroup = GetControl<UI_move_group>("move_group");
        //     _shootGroup = GetControl<UI_shoot_group>("shoot_group");
        //     _imgHead = GetControl<Image>("img_head");
        //     _imgHeadEnemy = GetControl<Image>("img_head_enemy");
        //     _sldHp = GetControl<Slider>("sld_hp");
        //     _sldTargetHp = GetControl<Slider>("sld_target_hp");
        //     _txtHp = GetControl<Text>("txt_hp");
        //     _txtName = GetControl<Text>("txt_name");
        //     _txtRound = GetControl<Text>("txt_round");
        //     PanelEntity.SetCustomFade(0);
        // }
        //
        // protected internal override void OnShow(object userData)
        // {
        //     base.OnShow(userData);
        //     _bm = FsmMgr.Instance.GetFsm<BattleMaster>(BattleConst.BATTLE_FSM).Owner;
        //     Visible = false;
        // }
        //
        // protected internal override void OnCover()
        // {
        //     base.OnCover();
        //     Visible = false;
        //     EventMgr.Instance.RemoveListener(ClientEvent.BATTLE_ROLE_AIM, OnAim);
        //     EventMgr.Instance.RemoveListener(ClientEvent.BATTLE_ROLE_RELOAD, OnReload);
        // }
        //
        // protected internal override void OnRefocus(object userData)
        // {
        //     base.OnRefocus(userData);
        //     Visible = true;
        //     PanelEntity.Fade(true);
        //     if (userData is BattleRoleEntity role)
        //     {
        //         _battleRole = role;
        //         if(!_battleRole.IsEnemy)
        //             _roleInfo = RoleMgr.Instance.GetRole(((BattleStudentEntity)_battleRole).Student.roleId);
        //         UpdateView();
        //     }
        //     EventMgr.Instance.AddListener(ClientEvent.BATTLE_ROLE_AIM, OnAim);
        //     EventMgr.Instance.AddListener(ClientEvent.BATTLE_ROLE_RELOAD, OnReload);
        // }
        //
        // protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        // {
        //     base.OnUpdate(elapseSeconds, realElapseSeconds);
        //     if(!Visible) return;
        //     if (!IsAim)
        //         _moveGroup.OnMove();
        //     
        //     if (_sldTargetHp.gameObject.activeSelf && _target != null)
        //     {
        //         _sldTargetHp.value = Mathf.Lerp(_sldTargetHp.value, _target.CurHp, Time.deltaTime * 3);
        //
        //         //设置显示位置
        //         var targetPos = _target.followTarget.position;
        //         var screenPos = _bm.cam.WorldToScreenPoint(targetPos + Vector3.up * 1);
        //         var trans = (RectTransform)_sldTargetHp.transform;
        //         float distance = Vector3.Distance(targetPos + Vector3.up * 1, _bm.cam.transform.position);
        //         trans.position = (Vector2)screenPos;
        //         trans.localScale = Mathf.Clamp(1 - distance * sizeDelta, 0.3f, 1f) * Vector3.one;
        //     }
        // }
        //
        // protected internal override void OnHide(bool isShutdown, object userData)
        // {
        //     base.OnHide(isShutdown, userData);
        //     EventMgr.Instance.RemoveListener(ClientEvent.BATTLE_ROLE_AIM, OnAim);
        //     EventMgr.Instance.RemoveListener(ClientEvent.BATTLE_ROLE_RELOAD, OnReload);
        // }
        //
        // private void UpdateView()
        // {
        //     if (_battleRole.IsEnemy)
        //     {
        //         var enemy = (BattleEnemyEntity)_battleRole;
        //         _txtName.text = enemy.EnemyInfo.roleName;
        //         _imgHeadEnemy.gameObject.SetActive(true);
        //         _imgHead.gameObject.SetActive(false);
        //         ResMgr.Instance.LoadAsync<Sprite>(GameConst.RES_GROUP_UI, ResPath.GetEnemyIcon(enemy.EnemyInfo.roleName), img =>
        //         {
        //             _imgHead.sprite = img;
        //         });
        //         _txtRound.text = "<color=red>敌方回合</color>";
        //     }
        //     else
        //     {
        //         _txtName.text = _roleInfo.fullName.Split(' ')[1];
        //         _imgHeadEnemy.gameObject.SetActive(false);
        //         _imgHead.gameObject.SetActive(true);
        //         ResMgr.Instance.LoadAsync<Sprite>(GameConst.RES_GROUP_UI, ResPath.GetStudentIcon(_roleInfo), img =>
        //         {
        //             _imgHead.sprite = img;
        //         });
        //         _txtRound.text = "<color=blue>我方回合</color>";
        //     }
        //     
        //     _sldTargetHp.gameObject.SetActive(false);
        //     _sldHp.maxValue = _battleRole.MaxHp;
        //     _sldHp.value = _battleRole.CurHp;
        //     _txtHp.text = $"{_battleRole.CurHp}/{_battleRole.MaxHp}";
        //     IsAim = false;
        //     _moveGroup.SetData(_battleRole);
        //     _shootGroup.SetData(_battleRole);
        // }
        //
        // private void OnAim()
        // {
        //     IsAim = true;
        // }
        //
        // private void OnReload()
        // {
        //     _shootGroup.SetData(_battleRole);
        // }
        //
        // public void SwitchArrow(bool isFocus) => _shootGroup.SwitchArrow(isFocus);
        //
        // public void ShowTargetHp(BattleRoleEntity role)
        // {
        //     _target = role;
        //     _sldTargetHp.maxValue = role.MaxHp;
        //     _sldTargetHp.value = role.CurHp;
        //     _sldTargetHp.gameObject.SetActive(true);
        // }
        //
        // public void HideTargetHp()
        // {
        //     _sldTargetHp.gameObject.SetActive(false);
        // }
        //
        // public void SetFlagTipActive(bool isActive)
        // {
        //     _moveGroup.SetFlagTip(isActive);
        // }
    }
}