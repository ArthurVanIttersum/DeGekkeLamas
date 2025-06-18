using UnityEngine;
using UnityEngine.UI;

public class GridDisplay : MonoBehaviour
{
    public MatchGridSystem grid;
    
    Image[,] UIImages;
    
    void UpdateDisplay()
    {
        for (int i = 0; i < grid.gridDimensions.x; i++)
        {
            for (int j = 0; j < grid.gridDimensions.y; j++)
            {
                //UIImages[i,j].sprite = grid.currentGrid[i, j].Sprite;
                //todo fix conversion issue later.
            }
        }

    }

    
}
