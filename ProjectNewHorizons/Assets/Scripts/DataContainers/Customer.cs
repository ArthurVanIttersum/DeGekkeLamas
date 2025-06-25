using UnityEngine;

public class Customer : MonoBehaviour
{
    public Order thisCustomersOrder;
    [HideInInspector] public int index;

    private void Start()
    {
        //transform.LookAt(Camera.main.transform.position);
        //transform.eulerAngles += new Vector3(0, 180, 0);
        this.transform.eulerAngles = Camera.main.transform.eulerAngles;
    }
}
