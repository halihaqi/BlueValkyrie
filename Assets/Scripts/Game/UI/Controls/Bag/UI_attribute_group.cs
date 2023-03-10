using Game.Managers;
using Game.Model;
using Hali_Framework;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public class UI_attribute_group : ControlBase
    {
        private Text _txtHp;
        private Text _txtAp;
        private Text _txtAtk;
        private Text _txtDef;

        private StudentItem _student;

        protected internal override void OnInit()
        {
            base.OnInit();
            _txtHp = GetControl<Text>("txt_hp");
            _txtAp = GetControl<Text>("txt_ap");
            _txtAtk = GetControl<Text>("txt_atk");
            _txtDef = GetControl<Text>("txt_def");
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            EventMgr.Instance.RemoveListener(ClientEvent.EXP_UP, UpdateView);
            EventMgr.Instance.RemoveListener(ClientEvent.WEAR_EQUIP, UpdateView);
            EventMgr.Instance.RemoveListener<int>(ClientEvent.LEVEL_PRE_UP, OnLvPreUp);
            EventMgr.Instance.RemoveListener(ClientEvent.STAR_PRE_UP, OnStarPreUp);
        }

        public void SetData(StudentItem student)
        {
            EventMgr.Instance.AddListener(ClientEvent.EXP_UP, UpdateView);
            EventMgr.Instance.AddListener(ClientEvent.WEAR_EQUIP, UpdateView);
            EventMgr.Instance.AddListener<int>(ClientEvent.LEVEL_PRE_UP, OnLvPreUp);
            EventMgr.Instance.AddListener(ClientEvent.STAR_PRE_UP, OnStarPreUp);

            _student = student;
            UpdateView();
        }

        private void UpdateView()
        {
            _txtHp.text = _student.Hp.ToString();
            _txtAp.text = _student.Ap.ToString();
            _txtAtk.text = _student.Atk.ToString();
            _txtDef.text = _student.Def.ToString();
        }

        public void OnLvPreUp(int toLvl)
        {
            if (toLvl == 0)
            {
                UpdateView();
                return;
            }
            
            int addHp = 0, addAp = 0, addAtk = 0, addDef = 0;
            
            var lvlList = StudentMgr.Instance.GetLevelUpList();
            for (int i = _student.lv - 1; i < toLvl; i++)
            {
                addHp += lvlList[i].addHp;
                addAp += lvlList[i].addAp;
                addAtk += lvlList[i].addAtk;
                addDef += lvlList[i].addDef;
            }
            
            _txtHp.text = $"{_student.Hp}<color=green>(+{addHp})</color>";
            _txtAp.text = $"{_student.Ap}<color=green>(+{addAp})</color>";
            _txtAtk.text = $"{_student.Atk}<color=green>(+{addAtk})</color>";
            _txtDef.text = $"{_student.Def}<color=green>(+{addDef})</color>";
        }

        private void OnStarPreUp()
        {
            var info = StudentMgr.Instance.GetStarUpInfo(_student.star);
            _txtHp.text = $"{_student.Hp}<color=green>(+{info.addHp})</color>";
            _txtAp.text = $"{_student.Ap}<color=green>(+{info.addAp})</color>";
            _txtAtk.text = $"{_student.Atk}<color=green>(+{info.addAtk})</color>";
            _txtDef.text = $"{_student.Def}<color=green>(+{info.addDef})</color>";
        }
    }
}