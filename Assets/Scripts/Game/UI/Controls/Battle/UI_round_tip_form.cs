using System;
using System.Collections.Generic;
using DG.Tweening;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Controls.Battle
{
    public enum RoundTipType
    {
        StudentRound,
        EnemyRound,
        StudentWin,
        EnemyWin,
    }
    
    public class UI_round_tip_form : ControlBase
    {
        [SerializeField]
        private Color studentColor;
        [SerializeField]
        private Color enemyColor;

        private Text _txtTip;

        private static Dictionary<RoundTipType, string> _tipDic = new Dictionary<RoundTipType, string>
        {
            { RoundTipType.StudentRound, "己方回合" },
            { RoundTipType.EnemyRound, "敌方回合" },
            { RoundTipType.StudentWin, "己方胜利" },
            { RoundTipType.EnemyWin, "敌方胜利" },
        };

        protected internal override void OnInit()
        {
            base.OnInit();
            _txtTip = GetControl<Text>("txt_tip");
        }

        public void ShowTip(RoundTipType tipType, Action callback)
        {
            _txtTip.text = _tipDic[tipType];
            switch (tipType)
            {
                case RoundTipType.StudentRound:
                case RoundTipType.StudentWin:
                    _txtTip.color = studentColor;
                    break;
                case RoundTipType.EnemyRound:
                case RoundTipType.EnemyWin:
                    _txtTip.color = enemyColor;
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