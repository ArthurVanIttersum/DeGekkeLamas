using System.Collections.Generic;
using UnityEngine;

public static class MathTools
{
    /// <summary>
    /// Adds up all elements of an array together
    /// </summary>
    public static int ArrayTotal(int[] array)
    {
        int total = 0;
        foreach (var item in array)
        {
            total += item;
        }
        return total;
    }
    /// <summary>
    /// Adds up all elements of an array together
    /// </summary>
    public static float ArrayTotal(float[] array)
    {
        float total = 0;
        foreach (var item in array)
        {
            total += item;
        }
        return total;
    }
    /// <summary>
    /// Remaps a certain range into anither one
    /// </summary>
    public static float Remap(float oldRangeX, float oldRangeY, float newRangeX, float newRangeY, float value)
    {
        value = Mathf.InverseLerp(oldRangeX, oldRangeY, value);
        return Mathf.Lerp(newRangeX, newRangeY, value);
    }

    /// <summary>
    /// Returns a list of every child of this gameobject, also gives children of children
    /// </summary>
    public static List<GameObject> GetAllChildren(GameObject origin)
    {
        List<GameObject> found = new();

        for (int i = 0; i < origin.transform.childCount ; i++)
        {
            GameObject child = origin.transform.GetChild(i).gameObject;
            found.Add(child);

            foreach(GameObject deepChild in GetAllChildren(child))
            {
                found.Add(deepChild);
            }
        }

        return found;
    }

}
