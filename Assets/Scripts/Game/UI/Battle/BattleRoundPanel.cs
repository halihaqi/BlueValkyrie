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
        private bool _isFirstRound;
        
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
            EventMgr.Instance.AddListener(ClientEvent.BATTLE_HALF_ROUND_OVER, OnHalfRoundOver);
            _isFirstRound = true;
        }
        
        protected internal override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            EventMgr.Instance.RemoveListener<IBattleRole>(ClientEvent.CHESS_CLICK, OnChessClick);
            EventMgr.Instance.RemoveListener(ClientEvent.BATTLE_ROUND_RUN, OnRoundRun);
            EventMgr.Instance.RemoveListener(ClientEvent.BATTLE_HALF_ROUND_OVER, OnHalfRoundOver);
            _isFirstRound = false;
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
            UpdateView(_bm.RoundEngine);
            if (_isFirstRound)
            {
                _imgMask.gameObject.SetActive(true);
                _roundTipForm.ShowTip(RoundTipType.BattleStart, () =>
                {
                    if (_bm.RoundEngine.IsEnemy)
                        StartCoroutine(AutoRound());
                    else
                        _imgMask.gameObject.SetActive(false);
                });
                _isFirstRound = false;
                return;
            }
        
            _imgMask.gameObject.SetActive(true);
            if (_bm.RoundEngine.IsEnemy)
                StartCoroutine(AutoRound());
            else
                _imgMask.gameObject.SetActive(false);
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
            UpdateView(_bm.RoundEngine);
        }
        
        private void OnHalfRoundOver()
        {
            _imgMask.gameObject.SetActive(true);
            RoundTipType type = _bm.RoundEngine.IsEnemy ? RoundTipType.EnemyRound : RoundTipType.StudentRound;
            _roundTipForm.ShowTip(type, () =>
            {
                if (_bm.RoundEngine.IsEnemy)
                    StartCoroutine(AutoRound());
                else
                    _imgMask.gameObject.SetActive(false);
            });
        }
        
        private void UpdateView(RoundEngine roundEngine)
        {
            _txtRound.text = $"{roundEngine.CurRound}/{roundEngine.MaxRound}";
            _txtCamp.text = roundEngine.IsEnemy ? "<color=red>Enemy</color>" : "<color=blue>Student</color>";
            OnChessClick(null);
            _imgCp.rectTransform.sizeDelta = new Vector2(CP_WIDTH * roundEngine.CurAp, _imgCp.rectTransform.sizeDelta.y);
        }
        
        private void OnBtnOverClick()
        {
            _bm.RoundEngine.HalfOver();
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