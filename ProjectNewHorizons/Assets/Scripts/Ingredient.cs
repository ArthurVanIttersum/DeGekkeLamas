using UnityEngine;


[System.Serializable]
public struct Ingredient // Contains ingredient data
{
    public string name;
    public Material material;
    [HideInInspector] public int index;

    public bool IndexEquals(Ingredient other)
    {
        return other.index == index;
    }
    public bool NameEquals(Ingredient other)
    {
        return other.name == name;
    }
    public static int FindByName(Ingredient[] ingredients, string name)
    {
        foreach(Ingredient ingredient in ingredients)
        {
            if (ingredient.name == name) return ingredient.index;
        }
        return 0;
    }
}
