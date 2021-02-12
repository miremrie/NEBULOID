using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.Input
{
    public static class InputUtils
    {
        public static int AxisToInt(float axisValue, float deadzoneValue)
        {
            if (Mathf.Abs(axisValue) >= Mathf.Abs(deadzoneValue))
            {
                return (int)Mathf.Sign(axisValue);
            }
            else
            {
                return 0;
            }
        }
        public static Vector2Int Axis2DToInt(Vector2 axis2D, float deadzoneValue)
        {
            return new Vector2Int(AxisToInt(axis2D.x, deadzoneValue), AxisToInt(axis2D.y, deadzoneValue));
        }
    }
}

