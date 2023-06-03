using System;
using System.Collections.Generic;
using DG.Tweening;
using Game.Model;
using Hali_Framework;
using UnityEngine;

namespace Game.UI.Controls
{
    public enum RoundTipType
    {
        Null,
        StudentRound,
        EnemyRound,
        StudentWin,
        EnemyWin,
        BattleStart,
    }
    
    public partial class UI_round_tip_form : ControlBase
    {
        [SerializeField]
        private Color studentColor;
        [SerializeField]
        private Color enemyColor;

        private static Dictionary<RoundTipType, string> _tipDic = new Dictionary<RoundTipType, string>
        {
            { RoundTipType.Null, "" },
            { RoundTipType.StudentRound, "己方回合" },
            { RoundTipType.EnemyRound, "敌方回合" },
            { RoundTipType.StudentWin, "己方胜利" },
            { RoundTipType.EnemyWin, "敌方胜利" },
            { RoundTipType.BattleStart, "战斗开始" }
        };

        public void ShowTip(RoundTipType tipType, Action callback)
        {
            txt_tip.text = _tipDic[tipType];
            switch (tipType)
            {
                case RoundTipType.StudentRound:
                case RoundTipType.StudentWin:
                    txt_tip.color = studentColor;
                    break;
                case RoundTipType.EnemyRound:
                case RoundTipType.EnemyWin:
                    txt_tip.color = enemyColor;
                    break;
            }
            DOTween.To(() => canvasGroup.alpha, 
                val => canvasGroup.alpha = val, 1, 1f);
            DelayUtils.Instance.Delay(2, 1, obj =>
            {
                DOTween.To(() => canvasGroup.alpha, 
                    val => canvasGroup.alpha = val, 0, 1f);
                callback?.Invoke();
            });
        }

        public RoundTipType GetTipType(RoleType type)
        {
            switch (type)
            {
                case RoleType.Null:
                    return RoundTipType.BattleStart;
                case RoleType.Student:
                    return RoundTipType.StudentRound;
                case RoleType.Enemy:
                    return RoundTipType.EnemyRound;
                default:
                    return RoundTipType.Null;
            }
        }
    }
}