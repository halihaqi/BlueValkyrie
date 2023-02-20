using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.UI.Utils
{
    [ExecuteInEditMode]
    public class RingLayoutGroup : MonoBehaviour
    {
        //半径
        public float radius = 250;
        
        private RectTransform _rect;
        public void ArcBtnGroup()
        {
            var btns = this.transform.GetComponentsInChildren<Button>();
            //平分角度
            float angel = Mathf.PI / (btns.Length + 1) * Mathf.Rad2Deg;
            
            //朝顶点向量
            Vector3 vector = this.transform.up * radius;

            foreach (var btn in btns)
            {
                vector = Quaternion.AngleAxis(angel, Vector3.forward) * vector;
                btn.transform.position = vector + this.transform.position;
            }
        }

        private void Awake()
        {
            if(Application.isPlaying)
                ArcBtnGroup();
        }

        private void OnValidate()
        {
            if(!Application.isPlaying)
                ArcBtnGroup();
        }
    }
}