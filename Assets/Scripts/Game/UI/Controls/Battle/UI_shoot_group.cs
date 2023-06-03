using System;
using Hali_Framework;
using UnityEngine;

namespace Game.UI.Controls
{
    public partial class UI_shoot_group : ControlBase
    {
        private Color _alphaZero = new Color(1, 1, 1, 0);
        private Color _alphaNormal = new Color(1, 1, 1, 1);
        
        public void SetData(Action fireCb)
        {
            tip_fire.SetData(KeyCode.F, "开火", fireCb);
        }

        public void ChangeAimMode(bool isTargetIn)
        {
            img_normal_point.color = isTargetIn ? _alphaZero : _alphaNormal;
            img_focus_point.color = isTargetIn ? _alphaNormal : _alphaZero;
        }
    }
}