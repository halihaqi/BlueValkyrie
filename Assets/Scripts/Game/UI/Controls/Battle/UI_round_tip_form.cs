﻿using System;
using System.Collections.Generic;
using DG.Tweening;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Controls
{
    public enum RoundTipType
    {
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
            { RoundTipType.StudentRound, "己方回合" },
            { RoundTipType.EnemyRound, "敌方回合" },
            { RoundTipType.StudentWin, "己方胜利" },
            { RoundTipType.EnemyWin, "敌方胜利" },
            { RoundTipType.BattleStart, "游戏开始" }
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
    }
}