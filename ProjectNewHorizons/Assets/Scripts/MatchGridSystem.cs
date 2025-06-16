using UnityEngine;

[System.Serializable]
public struct Ingredient // Contains ingredient data
{
    public string Name;
    public Material material;
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

    /// <summary>
    /// Randomize grid for connect 3
    /// </summary>
    void GenerateGrid()
    {
        for (int y = 0; y < currentGrid.GetLength(0); y++)
        {
            for(int x = 0; x < currentGrid.GetLength(1); x++)
            {

            }
        }
    }
}
