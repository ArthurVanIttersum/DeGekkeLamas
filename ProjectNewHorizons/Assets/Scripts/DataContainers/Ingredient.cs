using UnityEngine;


[System.Serializable]
public struct Ingredient // Contains ingredient data
{
    public string name;
    public Texture texture;
    public Material material;
    [HideInInspector] public int index;
    public GameObject cubeForDisplay;

    /// <summary>
    /// Checks if 2 ingredients are the same by using their hidden index
    /// </summary>
    public bool IndexEquals(Ingredient other)
    {
        return other.index == index;
    }

    /// <summary>
    /// Checks if 2 ingredients are the same using their names
    /// </summary>
    public bool NameEquals(Ingredient other)
    {
        return other.name == name;
    }

    /// <summary>
    /// Returns the hidden index of an ingredient in a list using its name
    /// </summary>
    public static int FindByName(Ingredient[] ingredients, string name)
    {
        foreach(Ingredient ingredient in ingredients)
        {
            if (ingredient.name == name) return ingredient.index;
        }
        return 0;
    }
}
