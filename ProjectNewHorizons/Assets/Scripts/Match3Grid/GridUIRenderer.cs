using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class GridUIRenderer : MonoBehaviour
{
    public MatchGridSystem matchGridSystem;
    public RectTransform gridParent;
    [Range(0f,1f)] public float borderFraction;
    public RawImage image;

    private List<RawImage> generated = new();

    /// <summary>
    /// Generates UI render of grid
    /// </summary>
    public void GenerateUI()
    {
        DestroyOldDisplay();
        gridParent.gameObject.SetActive(true);

        float percent = 1f / matchGridSystem.gridDimensions.x;
        Vector2 size = gridParent.sizeDelta;
        Vector2 blocksize = size * percent;

        Vector2 spawnPos = new Vector2(gridParent.position.x, gridParent.position.y) - size * .5f + (.5f * blocksize);
        Vector2 originalPos = spawnPos;
        for (int y = 0; y < matchGridSystem.currentGrid.GetLength(0); y++)
        {
            for (int x = 0; x < matchGridSystem.currentGrid.GetLength(1); x++)
            {
                RawImage spawnedSprite = Instantiate(image, new(), Quaternion.identity, gridParent.transform);
                spawnedSprite.rectTransform.sizeDelta = blocksize;
                spawnedSprite.rectTransform.position = spawnPos;
                spawnPos.x += blocksize.x;
                spawnedSprite.texture = matchGridSystem.currentGrid[y,x].texture;
                spawnedSprite.gameObject.GetOrAddComponent<GridPosition>().index = new(x, y);
                generated.Add(spawnedSprite);
            }
            spawnPos.x = originalPos.x;
            spawnPos.y += blocksize.y;
        }
        print("Generated UI display");
    }

    void DestroyOldDisplay()
    {
        for(int i = 0; i < generated.Count; i++)
        {
            Destroy(generated[i].gameObject);
        }
        generated.Clear();
        print("Cleared old display");
    }

    void ExitScreen()
    {
        gridParent.gameObject.SetActive(false);
    }
}
