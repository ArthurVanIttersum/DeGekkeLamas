using UnityEngine;

public static class MathTools
{
    public static int ArrayTotal(int[] array)
    {
        int total = 0;
        foreach (var item in array)
        {
            total += item;
        }
        return total;
    }
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
