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

}
