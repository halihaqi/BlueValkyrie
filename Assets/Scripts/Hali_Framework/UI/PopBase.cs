using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hali_Framework
{
    [RequireComponent(typeof(Image))]
    public abstract class PopBase : PanelBase
    {
        protected Image bk;
        protected bool isModal = false;
        protected bool isTouchHide = true;

        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);
            bk = GetComponent<Image>();
            if(isModal && isTouchHide && bk != null)
                AddCustomListener(bk, EventTriggerType.PointerClick, data =>
                {
                    HideMe();
                });
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
            if (isModal && bk != null)
            {
                bk.enabled = true;
                bk.raycastTarget = true;
            }
        }
    }
}