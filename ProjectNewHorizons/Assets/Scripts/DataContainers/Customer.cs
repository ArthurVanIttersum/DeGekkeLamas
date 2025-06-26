using UnityEngine;

public class Customer : MonoBehaviour
{
    public Order thisCustomersOrder;
    [HideInInspector] public int index;

    private void Start()
    {
        this.transform.eulerAngles = Camera.main.transform.eulerAngles;
    }
}
