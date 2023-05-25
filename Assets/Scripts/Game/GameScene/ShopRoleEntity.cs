using System;
using Game.Base;
using Game.Managers;
using Game.Model;
using Game.UI.Base;
using Game.UI.Game;
using Hali_Framework;
using UnityEngine;

namespace Game.Entity
{
    public class ShopRoleEntity : NPCBase
    {
        protected override void OnDialogueOver()
        {
            base.OnDialogueOver();
            UIMgr.Instance.ShowPanel<ShopPanel>();
        }
    }
}