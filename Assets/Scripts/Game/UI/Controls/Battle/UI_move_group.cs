using System;
using Game.BattleScene.BattleRole;
using Hali_Framework;
using UnityEngine;

namespace Game.UI.Controls
{
    public partial class UI_move_group : ControlBase
    {
        public void SetData(IBattleRole role, Action aimCb, Action mapCb)
        {
            sld_ap.maxValue = role.RoleState.maxAp;
            sld_ap.value = role.CurAp;
            tip_aim.SetData(KeyCode.R, "瞄准模式", aimCb);
            tip_map.SetData(KeyCode.Tab, "地图", mapCb);
        }
    }
}