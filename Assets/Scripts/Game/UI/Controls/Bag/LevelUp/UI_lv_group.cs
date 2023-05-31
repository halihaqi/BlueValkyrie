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

namespace Game.UI.Controls
{
    public partial class UI_lv_group : ControlBase
    {
        private Dictionary<int, ExpInfo> _expDic;
        private List<BagItemInfo> _bagExpList;
        private Dictionary<ExpInfo, int> _addExps;//添加的经验书
        private StudentItem _student;
        private int _curCost;

        protected internal override void OnInit()
        {
            base.OnInit();
            sv_lv.itemRenderer = OnItemRender;
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
            txt_cost.text = _curCost.ToString();
            btn_lv.SetData("升级", OnUpClick);
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
                txt_cost.text = _curCost.ToString();
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
            txt_cost.text = _curCost.ToString();
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
            sld_exp_add.value = Math.Min(addExp + sld_exp.value, sld_exp_add.maxValue);
            var str = txt_exp.text.Split('/');
            txt_exp.text = $"{addExp + sld_exp.value}/{str[1]}";
            
            //更新花费金额和按钮
            _curCost = cost;
            if (PlayerMgr.Instance.BagMaster.TryGetItem(0, 1, out var item) && item.num >= cost)
            {
                txt_cost.text = cost.ToString();
                btn_lv.SetGray(false);
            }
            else
            {
                txt_cost.text = $"<color=red>{cost}</color>";
                btn_lv.SetGray(true);
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
                txt_lv.text = $"<color=green>{upLvl.ToLv()}</color>";
                EventMgr.Instance.TriggerEvent(ClientEvent.LEVEL_PRE_UP, upLvl);
            }
            else
            {
                txt_lv.text = _student.lv.ToLv();
                EventMgr.Instance.TriggerEvent(ClientEvent.LEVEL_PRE_UP, 0);
            }
        }

        private void UpdateExp()
        {
            txt_lv.text = _student.lv.ToLv();
            var maxExp = StudentMgr.Instance.GetLvlUpExp(_student.lv);
            txt_exp.text = $"{_student.exp}/{maxExp}";
            sld_exp_add.maxValue = sld_exp.maxValue = maxExp;
            sld_exp_add.value = sld_exp.value = _student.exp;
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
            sv_lv.numItems = _bagExpList.Count;
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