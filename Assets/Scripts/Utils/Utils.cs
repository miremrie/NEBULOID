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
    public static int SumAllowFlow(int a, int b, int min, int max)
    {
        int sum = a + b;
        if (sum >= min && sum <= max) return sum;
        else if (sum > max) return min + ((sum - max - 1) % (max + 1 - min));
        else return max + ((sum - min + 1) % (max + 1 - min));
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

}

public static class Vectors
{
    public static Vector2 ToVector2(this Vector3 v) => new Vector2(v.x, v.y);
}


internal class MaybeNot : System.Exception { }
public struct Maybe<T>
{
    private T item;
    public bool Valid { get; private set; }

    public T Item
    {
        get
        {
            if (!Valid) throw new MaybeNot();
            else return item;
        }

        set
        {
            item = value;
            Valid = true;
        }
    }

    public Maybe(T item, bool isValid = true)
    {
        this.item = item;
        this.Valid = isValid;
    }
}
