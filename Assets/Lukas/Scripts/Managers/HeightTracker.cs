using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class HeightTracker
{
    public static HashSet<GameObject> z0 = new();
    public static HashSet<GameObject> z1 = new();
    public static HashSet<GameObject> z2 = new();

    public static bool AddToZLayer(GameObject obj, float lvl)
    {
        switch (lvl)
        {
            case 0:
                return z0.Add(obj);
            case 1:
                return z1.Add(obj);
            case 2:
                return z2.Add(obj);
            default:
                Debug.LogError("Invalid value lvl: " + obj.name + " not added to height tracker");
                return false;
        }
    }

    public static bool RemoveFromZLayer(GameObject obj, float lvl)
    {
        switch (lvl)
        {
            case 0:
                return z0.Remove(obj);
            case 1:
                return z1.Remove(obj);
            case 2:
                return z2.Remove(obj);
            default:
                Debug.LogError("Invalid value lvl: " + obj.name + " not added to height tracker");
                return false;
        }
    }

    // Returns a boolean array with length 3 indicating which z layers GameObject is part of
    public static bool[] GetZ(GameObject obj)
    {
        return new bool[] { z0.Contains(obj), z1.Contains(obj), z2.Contains(obj) };

    }


}
