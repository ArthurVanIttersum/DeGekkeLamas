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

}
