using System.Collections;
using Game.BattleScene;
using Game.BattleScene.BattleRole;
using Game.Model;
using Game.UI.Controls;
using Hali_Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI.Battle
{
    public class BattleRoundPanel : PanelBase
    {
        [SerializeField] private RectTransform chessTrans;
        private const int CP_WIDTH = 167;

        private Image _imgCp;
        private RawImage _rawMap;
        private Text _txtRound;
        private Text _txtCamp;
        private UI_battle_role_form _battleRoleForm;
        private UI_round_tip_form _roundTipForm;
        private Image _imgMask;
        
        private Button _btnOver;
        private Button _btnStudent;
        private Button _btnOption;
        
        private BattleMaster _bm;

        public RectTransform Map => (RectTransform)_rawMap.transform;
        public RectTransform ChessTrans => chessTrans;
        
        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);
            _imgCp = GetControl<Image>("img_cp");
            _rawMap = GetControl<RawImage>("raw_map");
            _btnOver = GetControl<Button>("btn_over");
            _btnStudent = GetControl<Button>("btn_student");
            _btnOption = GetControl<Button>("btn_option");
            _txtRound = GetControl<Text>("txt_round");
            _txtCamp = GetControl<Text>("txt_camp");
            _battleRoleForm = GetControl<UI_battle_role_form>("battle_role_form");
            _roundTipForm = GetControl<UI_round_tip_form>("round_tip_form");
            _imgMask = GetControl<Image>("img_mask");
            
            _btnOver.onClick.AddListener(OnBtnOverClick);
            UIMgr.AddCustomEventListener(_rawMap, EventTriggerType.PointerClick, OnMapClick);
            PanelEntity.SetCustomFade(0);
        }
        
        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
            _bm = FsmMgr.Instance.GetFsm<BattleMaster>(BattleConst.BATTLE_FSM).Owner;
            Visible = false;
            EventMgr.Instance.AddListener<IBattleRole>(ClientEvent.CHESS_CLICK, OnChessClick);
            EventMgr.Instance.AddListener(ClientEvent.BATTLE_ROUND_RUN, OnRoundRun);
        }
        
        protected internal override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            EventMgr.Instance.RemoveListener<IBattleRole>(ClientEvent.CHESS_CLICK, OnChessClick);
            EventMgr.Instance.RemoveListener(ClientEvent.BATTLE_ROUND_RUN, OnRoundRun);
        }
        
        protected internal override void OnCover()
        {
            base.OnCover();
            Visible = false;
        }
        
        protected internal override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
            Visible = true;
            PanelEntity.Fade(true);
            UpdateView();
            
            _imgMask.gameObject.SetActive(true);
            _roundTipForm.ShowTip(_roundTipForm.GetTipType(_bm.CurRole.RoleType), () =>
            {
                _imgMask.gameObject.SetActive(false);
            });
        }

        private void OnChessClick(IBattleRole role)
        {
            if (role == null)
            {
                _battleRoleForm.SetAlpha(0);
                return;
            }
            
            _battleRoleForm.SetAlpha(1);
            _battleRoleForm.SetData(role, role.RoleType == RoleType.Student);
        }
        
        private void OnMapClick(BaseEventData data)
        {
            EventMgr.Instance.TriggerEvent<IBattleRole>(ClientEvent.CHESS_CLICK, null);
        }
        
        private void OnRoundRun()
        {
            UpdateView();
        }

        private void UpdateView()
        {
            _txtRound.text = $"{_bm.CurRound}/{_bm.MaxRound}";
            _txtCamp.text = _bm.CurCamp.type.ToString();
            OnChessClick(null);
            _imgCp.rectTransform.sizeDelta = new Vector2
                (CP_WIDTH * _bm.CurCamp.curAp, _imgCp.rectTransform.sizeDelta.y);
        }
        
        private void OnBtnOverClick()
        {
            _bm.WarCampOver();
        }
        
        private IEnumerator AutoRound()
        {
            //_bm.AutoEnemyIndex();
            yield return new WaitForSeconds(1);
            //点击敌方棋子
            EventMgr.Instance.TriggerEvent(ClientEvent.CHESS_AUTO_CLICK, _bm.CurRole);
            yield return new WaitForSeconds(1);
            
            EventMgr.Instance.TriggerEvent(ClientEvent.BATTLE_ROLE_ACTION, _bm.CurRole);
        }
    }
}