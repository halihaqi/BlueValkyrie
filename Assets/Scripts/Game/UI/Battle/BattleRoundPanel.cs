using System.Collections;
using Game.BattleScene;
using Game.Global;
using Game.UI.Controls.Battle;
using Hali_Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI.Battle
{
    public class BattleRoundPanel : PanelBase
    {
        private const int CP_WIDTH = 167;

        private Image _imgCp;
        private RawImage _rawMap;
        private Button _btnOver;
        private Button _btnStudent;
        private Button _btnOption;
        private Text _txtRound;
        private Text _txtCamp;
        private UI_battle_student_form _battleStudentForm;
        private UI_battle_enemy_form _battleEnemyForm;
        private UI_round_tip_form _roundTipForm;
        private Image _imgMask;

        private BattleMaster _bm;

        public RectTransform Map => (RectTransform)_rawMap.transform;

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
            _battleStudentForm = GetControl<UI_battle_student_form>("battle_student_form");
            _battleEnemyForm = GetControl<UI_battle_enemy_form>("battle_enemy_form");
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
        }

        protected internal override void OnCover()
        {
            base.OnCover();
            Visible = false;
            EventMgr.Instance.RemoveListener<bool, int>(ClientEvent.CHESS_CLICK, OnChessClick);
            EventMgr.Instance.RemoveListener(ClientEvent.BATTLE_ROUND_RUN, OnRoundRun);
            EventMgr.Instance.RemoveListener(ClientEvent.BATTLE_HALF_ROUND_OVER, OnHalfRoundOver);
        }

        protected internal override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
            Visible = true;
            PanelEntity.Fade(true);
            UpdateView(_bm.RoundEngine);
            
            _imgMask.gameObject.SetActive(true);
            if (_bm.RoundEngine.IsEnemy)
                StartCoroutine(AutoRound());
            else
                _imgMask.gameObject.SetActive(false);
            
            EventMgr.Instance.AddListener<bool, int>(ClientEvent.CHESS_CLICK, OnChessClick);
            EventMgr.Instance.AddListener(ClientEvent.BATTLE_ROUND_RUN, OnRoundRun);
            EventMgr.Instance.AddListener(ClientEvent.BATTLE_HALF_ROUND_OVER, OnHalfRoundOver);
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            EventMgr.Instance.RemoveListener<bool, int>(ClientEvent.CHESS_CLICK, OnChessClick);
            EventMgr.Instance.RemoveListener(ClientEvent.BATTLE_ROUND_RUN, OnRoundRun);
            EventMgr.Instance.RemoveListener(ClientEvent.BATTLE_HALF_ROUND_OVER, OnHalfRoundOver);
            UIMgr.RemoveCustomEventListener(_rawMap, EventTriggerType.PointerClick, OnMapClick);
        }

        private void OnChessClick(bool isEnemy, int roleIndex)
        {
            if (roleIndex < 0)
            {
                _battleStudentForm.gameObject.SetActive(false);
                _battleEnemyForm.gameObject.SetActive(false);
                return;
            }
            
            _battleStudentForm.gameObject.SetActive(!isEnemy);
            _battleEnemyForm.gameObject.SetActive(isEnemy);
            if (isEnemy)
                _battleEnemyForm.SetData(roleIndex);
            else
                _battleStudentForm.SetData(roleIndex);
        }

        private void OnMapClick(BaseEventData data)
        {
            EventMgr.Instance.TriggerEvent(ClientEvent.CHESS_CLICK, false, -1);
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
            OnChessClick(false, -1);
            _imgCp.rectTransform.sizeDelta = new Vector2(CP_WIDTH * roundEngine.CurAp, _imgCp.rectTransform.sizeDelta.y);
        }

        private void OnBtnOverClick()
        {
            _bm.RoundEngine.HalfOver();
        }

        private IEnumerator AutoRound()
        {
            _bm.AutoEnemyIndex();
            yield return new WaitForSeconds(1);
            //点击敌方棋子
            EventMgr.Instance.TriggerEvent(ClientEvent.CHESS_AUTO_CLICK, true, _bm.CurRole.RoleIndex);
            yield return new WaitForSeconds(1);
            
            EventMgr.Instance.TriggerEvent(ClientEvent.BATTLE_ROLE_ACTION, _bm.CurRole);
        }
    }
}