using UnityEngine;

public class GridPosition : MonoBehaviour
{
    /// <summary>
    /// X value of this is 2nd index of the grid array, Y value of this is the 1st index
    /// </summary>
    public Vector2 index;
    public Vector3 destinationPosition;//to get the position of where it is moving to. used for switching aimation logic.
}
