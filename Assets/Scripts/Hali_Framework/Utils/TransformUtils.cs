using UnityEngine;

namespace Hali_Framework
{
    public static class TransformUtils
    {
        /// <summary>
        /// 限制自定义角度的范围到欧拉角
        /// (不可以限制localEulerAngles,因为它会自动修改)
        /// </summary>
        /// <param name="ifAngel"></param>
        /// <param name="ifMin"></param>
        /// <param name="ifMax"></param>
        /// <returns></returns>
        public static float ClampAngle(float ifAngel, float ifMin, float ifMax)
        {
            if (ifAngel < -360f)
                ifAngel += 360f;
            if (ifAngel > 360f)
                ifAngel -= 360f;
            return Mathf.Clamp(ifAngel, ifMin, ifMax);
        }
    }
}
