using System.Drawing;
using UnityEngine;

public class GridGameraController : MonoBehaviour
{
    public MatchGridSystem gridManager;
    
    public void SetCameraPositionAndScale()
    {
        Vector3 OffsetByHand = new Vector3(-0.5f, -0.5f, -1);
        Vector2Int gridsize = gridManager.gridDimensions;
        Vector2 size2D = (Vector2)gridsize;
        Vector3 size3D = new Vector3(size2D.x, size2D.y, 0);
        Vector3 Offset = size3D / 2;
        transform.position = gridManager.spawnPosition + Offset + OffsetByHand;
        gameObject.GetComponent<Camera>().orthographicSize = (float)gridsize.y / 2;
    }
}
