using Game.Base;
using UnityEngine;

namespace Game.Entity
{
    public class FormationRoleEntity : RoleBase
    {
        protected override void Awake()
        {
            base.Awake();
            //设置为UI层级
            var layer = LayerMask.NameToLayer("UI");
            SetLayerRecursively(this.gameObject, layer);
        }
    }
}