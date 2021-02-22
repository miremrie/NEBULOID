using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Colors
{
    public static Color RandomColor(float s, float v)
    {
        var hue = Random.Range(0f, 1f);
        return Color.HSVToRGB(hue, s, v);
    }
}
public class MMath
{
    public static bool LineSegmentsIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 intersection)
    {
        intersection = Vector2.zero;

        var d = (p2.x - p1.x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);

        if (d == 0.0f)
        {
            return false;
        }

        var u = ((p3.x - p1.x) * (p4.y - p3.y) - (p3.y - p1.y) * (p4.x - p3.x)) / d;
        var v = ((p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x)) / d;

        if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
        {
            return false;
        }

        intersection.x = p1.x + u * (p2.x - p1.x);
        intersection.y = p1.y + u * (p2.y - p1.y);

        return true;
    }

}
public static class LMath
{
    public static bool IsBetween(this float value, float border1, float border2)
    {
        return (value > border1 && value < border2) || (value > border2 && value < border1);
    }
    public static bool IsBetweenOrEqual(this float value, float border1, float border2)
    {
        return (value >= border1 && value <= border2) || (value >= border2 && value <= border1);
    }
    public static bool IsBetween(this int value, int border1, int border2)
    {
        return (value > border1 && value < border2) || (value > border2 && value < border1);
    }
    public static bool IsBetweenOrEqual(this int value, int border1, int border2)
    {
        return (value >= border1 && value <= border2) || (value >= border2 && value <= border1);
    }
    /*public static int AdditionOverflowKeepDirection(int current, int step, int min, int max)
    {
        int newValue = current + step;
        if (newValue > max)
        {
            int distanceFromEdge = Mathf.Abs(step) - (max - current);
            int distance = (Mathf.Abs(distanceFromEdge - max));
            int distanceMod = distance % (max - min);
            newValue = min + distanceMod;
        }
        else if (newValue < min)
        {
            int distanceFromEdge = step - (current - min);
            int distance = (Mathf.Abs(distanceFromEdge + min));
            int distanceMod = distance % (max - min);
            newValue = max + distanceMod;
        }
        return newValue;
    }*/
}
