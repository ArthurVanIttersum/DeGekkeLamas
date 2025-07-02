using UnityEngine;


public class MovingObject : MonoBehaviour
{
    public MovingObjectData stats;

    private void Awake()
    {
        stats.transform = this.transform;
        stats.Start();
    }
    private void FixedUpdate()
    {
        stats.DoMovement();
    }
}

[System.Serializable]
public struct MovingObjectData
{
    [Header("Movement related")]
    public float moveSpeedX;
    public float moveSpeedY;
    public float moveSpeedZ;
    [Header("Ranges")]
    public float moveRangeX;
    public float moveRangeY;
    public float moveRangeZ;
    [Header("Offset")]
    public float offsetX;
    public float offsetY;
    public float offsetZ;
    [Header("Loop")]
    public float distance;

    Vector3 _oriPos;
    [HideInInspector] public Transform transform;

    public enum MovementType { Circular, PingPong, Forward, Loop};
    public MovementType currentMovement;

    public void Start()
    {
        _oriPos = transform.localPosition;
    }

    public void DoMovement()
    {
        // Moves object
        switch (currentMovement)
        {
            case MovementType.PingPong:
                MovementPingPong();
                break;
            case MovementType.Circular:
                MovementCircular();
                break;
            case MovementType.Forward:
                MovementForward();
                break;
            case MovementType.Loop:
                MovementLoop();
                break;
        }
    }
    void MovementPingPong()
    {
        this.transform.localPosition = _oriPos + new Vector3(
                        Mathf.Sin(moveSpeedX * Time.time + offsetX) * moveRangeX,
                        Mathf.Sin(moveSpeedY * Time.time + offsetY) * moveRangeY,
                        Mathf.Sin(moveSpeedZ * Time.time + offsetZ) * moveRangeZ
                        );
    }
    void MovementCircular()
    {
        this.transform.localPosition = _oriPos + new Vector3(
                        Mathf.Sin(moveSpeedX * Time.time + offsetX) * moveRangeX,
                        Mathf.Sin(moveSpeedY * Time.time + offsetY) * moveRangeY,
                        Mathf.Cos(moveSpeedZ * Time.time + offsetZ) * moveRangeZ
                        );
    }
    void MovementForward()
    {
        this.transform.localPosition = _oriPos + new Vector3(
                        moveSpeedX * Time.time,
                        Mathf.Sin(moveSpeedY * Time.time) * moveRangeY,
                        moveSpeedZ * Time.time
                        );
    }
    void MovementLoop()
    {
        this.transform.localPosition = _oriPos + new Vector3(
                        (moveSpeedX * Time.time) % distance ,
                        (Mathf.Sin(moveSpeedY * Time.time) * moveRangeY) % distance,
                        (moveSpeedZ * Time.time) % distance
                        );
    }
}
