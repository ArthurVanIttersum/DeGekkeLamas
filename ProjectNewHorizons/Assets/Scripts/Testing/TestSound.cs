using UnityEngine;
using UnityEngine.Events;

public class TestSound : MonoBehaviour
{
    public UnityEvent theEventToActivate;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            theEventToActivate.Invoke();
        }
    }
}
