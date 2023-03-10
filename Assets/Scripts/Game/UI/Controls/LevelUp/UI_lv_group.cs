using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Game.Managers;
using Game.Model;
using Game.Model.BagModel;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Controls.LevelUp
{
    public class UI_lv_group : ControlBase
    {
        private Slider _sldExp;
        private Slider _sldExpAdd;
        private HList _svLv;
        private Text _txtLv;
        private Text _txtExp;
        private Text _txtCost;
        private UI_btn_big _btnLv;

        private Dictionary<int, ExpInfo> _expDic;
        private List<BagItemInfo> _bagExpList;
        private Dictionary<ExpInfo, int> _addExps;//添加的经验书
        private StudentItem _student;
        private int _curCost;

        protected internal override void OnInit()
        {
            base.OnInit();
            _sldExp = GetControl<Slider>("sld_exp");
            _sldExpAdd = GetControl<Slider>("sld_exp_add");
            _svLv = GetControl<HList>("sv_lv");
            _txtLv = GetControl<Text>("txt_lv");
            _txtExp = GetControl<Text>("txt_exp");
            _txtCost = GetControl<Text>("txt_cost");
            _btnLv = GetControl<UI_btn_big>("btn_lv");

            _svLv.itemRenderer = OnItemRender;
            _expDic = BinaryDataMgr.Instance.GetTable<ExpInfoContainer>().dataDic;
            _addExps = new Dictionary<ExpInfo, int>();
            _bagExpList = new List<BagItemInfo>(4);
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            EventMgr.Instance.RemoveListener<int, int>(ClientEvent.EXP_ADD, OnExpAdd);
            _addExps.Clear();
            _bagExpList.Clear();
        }

        public void SetData(StudentItem student)
        {
            EventMgr.Instance.AddListener<int, int>(ClientEvent.EXP_ADD, OnExpAdd);
            _student = student;
            _curCost = 0;
            _addExps.Clear();
            
            UpdateExp();
            UpdateExpItem();
            _txtCost.text = _curCost.ToString();
            _btnLv.SetData("升级", OnUpClick);
        }

        private void OnItemRender(int index, GameObject obj)
        {
            var item = obj.GetComponent<UI_lv_item>();
            item.SetData(_bagExpList[index]);
        }

        private void OnUpClick()
        {
            if(_addExps.Count <= 0) return;
            var bag = PlayerMgr.Instance.BagMaster;
            Dictionary<int, int> costDic = new Dictionary<int, int>();
            foreach (var exp in _addExps)
            {
                if (costDic.ContainsKey(exp.Key.costId))
                    costDic[exp.Key.costId] += exp.Key.cost * exp.Value;
                else
                    costDic.Add(exp.Key.costId, exp.Key.cost * exp.Value);
            }
            //检查库存是否够
            bool adequate = true;
            foreach (var cost in costDic)
            {
                if (!bag.HasItem(0, cost.Key))
                {
                    adequate = false;
                    break;
                }

                if (bag.GetItem(0, cost.Key).num < cost.Value)
                {
                    adequate = false;
                    break;
                }
            }

            //如果库存不足
            if (!adequate)
            {
                TipHelper.ShowTip("金币不足!");
                UpdateExp();
                UpdateExpItem();
                _curCost = 0;
                _txtCost.text = _curCost.ToString();
                return;
            }
            
            //消耗物品
            foreach (var cost in costDic)
                bag.AddItem(0, cost.Key, -cost.Value);
            foreach (var exp in _addExps)
                bag.AddItem(0, exp.Key.id, -exp.Value);

            //增加经验
            _student.AddExp(CalcExp());
            
            //更新面板
            UpdateExp();
            UpdateExpItem();
            _curCost = 0;
            _addExps.Clear();
            _txtCost.text = _curCost.ToString();
            EventMgr.Instance.TriggerEvent(ClientEvent.EXP_UP);
        }

        private void OnExpAdd(int expId, int num)
        {
            var expItem = _expDic[expId];
            if (_addExps.ContainsKey(expItem))
            {
                _addExps[expItem] += num;
                if (_addExps[expItem] <= 0)
                    _addExps.Remove(expItem);
            }
            else
                _addExps.Add(expItem, num);

            int cost = 0;
            foreach (var exp in _addExps)
                cost += exp.Key.cost * exp.Value;
            
            //更新UI
            //更新经验条
            int addExp = CalcExp();
            _sldExpAdd.value = Math.Min(addExp + _sldExp.value, _sldExpAdd.maxValue);
            var str = _txtExp.text.Split('/');
            _txtExp.text = $"{addExp + _sldExp.value}/{str[1]}";
            
            //更新花费金额和按钮
            _curCost = cost;
            if (PlayerMgr.Instance.BagMaster.TryGetItem(0, 1, out var item) && item.num >= cost)
            {
                _txtCost.text = cost.ToString();
                _btnLv.SetGray(false);
            }
            else
            {
                _txtCost.text = $"<color=red>{cost}</color>";
                _btnLv.SetGray(true);
            }

            //经验能够升的级数
            var lvlList = StudentMgr.Instance.GetLevelUpList();
            int allExp = _student.exp + addExp;
            int upLvl = 1;
            for (int i = 0; i < lvlList.Count; i++)
            {
                allExp -= lvlList[i].exp;
                if(allExp < 0) break;
                ++upLvl;
            }

            if (upLvl > _student.lv)
            {
                _txtLv.text = $"<color=green>{upLvl.ToLv()}</color>";
                EventMgr.Instance.TriggerEvent(ClientEvent.LEVEL_PRE_UP, upLvl);
            }
            else
            {
                _txtLv.text = _student.lv.ToLv();
                EventMgr.Instance.TriggerEvent(ClientEvent.LEVEL_PRE_UP, 0);
            }
        }

        private void UpdateExp()
        {
            _txtLv.text = _student.lv.ToLv();
            var maxExp = StudentMgr.Instance.GetLvlUpExp(_student.lv);
            _txtExp.text = $"{_student.exp}/{maxExp}";
            _sldExpAdd.maxValue = _sldExp.maxValue = maxExp;
            _sldExpAdd.value = _sldExp.value = _student.exp;
        }

        private void UpdateExpItem()
        {
            _bagExpList.Clear();
            var bagMaster = PlayerMgr.Instance.BagMaster;
            foreach (var kv in _expDic)
            {
                if (bagMaster.HasItem(0, kv.Key))
                    _bagExpList.Add(bagMaster.GetItem(0, kv.Key));
            }
            _svLv.numItems = _bagExpList.Count;
        }

        private int CalcExp()
        {
            int sum = 0;
            foreach (var exp in _addExps)
                sum += exp.Key.exp * exp.Value;
            return sum;
        }
    }
}