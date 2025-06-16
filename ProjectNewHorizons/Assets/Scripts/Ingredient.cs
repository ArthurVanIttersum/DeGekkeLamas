using UnityEngine;


[System.Serializable]
public struct Ingredient // Contains ingredient data
{
    public string Name;
    public Material material;
    [HideInInspector] public int index;

    public bool Equals(Ingredient other)
    {
        return other.index == index;
    }
}
