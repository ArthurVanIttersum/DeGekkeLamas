using UnityEngine;

[System.Serializable]
public struct Ingredient // Contains ingredient data
{
    public string Name;
    public Texture2D Sprite;
    [HideInInspector] public int index;
}

public class MatchGridSystem : MonoBehaviour
{
    public Ingredient[] ingredientTypes;
    public Vector2Int gridDimensions;
    Ingredient[,] currentGrid;
    private void OnValidate()
    {
        for(int i = 0; i < ingredientTypes.Length; i++) ingredientTypes[i].index = i;
    }

    void Initialize()
    {
        currentGrid = new Ingredient[gridDimensions.y, gridDimensions.x];
    }

    void GenerateGrid()
    {

    }
}
