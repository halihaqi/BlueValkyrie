using Game.Managers;
using Game.Model;
using Hali_Framework;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public partial class UI_attribute_group : ControlBase
    {
        private StudentItem _student;

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
            txt_hp.text = _student.Hp.ToString();
            txt_ap.text = _student.Ap.ToString();
            txt_atk.text = _student.Atk.ToString();
            txt_def.text = _student.Def.ToString();
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
            
            txt_hp.text = $"{_student.Hp}<color=green>(+{addHp})</color>";
            txt_ap.text = $"{_student.Ap}<color=green>(+{addAp})</color>";
            txt_atk.text = $"{_student.Atk}<color=green>(+{addAtk})</color>";
            txt_def.text = $"{_student.Def}<color=green>(+{addDef})</color>";
        }

        private void OnStarPreUp()
        {
            var info = StudentMgr.Instance.GetStarUpInfo(_student.star);
            txt_hp.text = $"{_student.Hp}<color=green>(+{info.addHp})</color>";
            txt_ap.text = $"{_student.Ap}<color=green>(+{info.addAp})</color>";
            txt_atk.text = $"{_student.Atk}<color=green>(+{info.addAtk})</color>";
            txt_def.text = $"{_student.Def}<color=green>(+{info.addDef})</color>";
        }
    }
}