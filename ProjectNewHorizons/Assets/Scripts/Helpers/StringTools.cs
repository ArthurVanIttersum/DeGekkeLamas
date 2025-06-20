using UnityEngine;

static public class StringTools
{
    /// <summary>
    /// Generate a string of the ingredient names
    /// </summary>
    static public string IngredientArrayToString(Ingredient[] array)
    {
        string full = string.Empty;

        full += array[0].name;
        for(int i = 1; i < array.Length; i++)
        {
            full += $"\n {array[i].name}";
        }

        return full;
    }
    static public string StrikeThrough(string str)
    {
        return $"<s>{str}</s>";
    }
    static public string StrikeThrough(string full, string part)
    {
        return full.Replace(part, $"<s>{part}</s>");
    }
    static public string Bold(string str)
    {
        return $"<b>{str}</b>";
    }
    static public string Bold(string full, string part)
    {
        return full.Replace(part, $"<b>{part}</b>");
    }
}
