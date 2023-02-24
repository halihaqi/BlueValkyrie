using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI.Utils
{
    [ExecuteInEditMode]
    public class RingLayoutGroup : UIBehaviour
    {
        //半径
        public float radius = 250;
        public void ArcBtnGroup()
        {
            RectTransform rect = this.transform as RectTransform;
            var btns = rect.GetComponentsInChildren<Button>();
            //平分角度
            float angel = Mathf.PI / (btns.Length + 1) * Mathf.Rad2Deg;
            
            //朝顶点向量
            Vector3 vector = rect.up * radius;

            foreach (var btn in btns)
            {
                vector = Quaternion.AngleAxis(angel, rect.forward) * vector;
                ((RectTransform)btn.transform).anchoredPosition =
                    (Vector2)vector + rect.anchoredPosition;
            }
        }

        protected override void Awake()
        {
            if(Application.isPlaying)
                ArcBtnGroup();
        }

        protected override void OnValidate()
        {
            ArcBtnGroup();
        }
    }
}