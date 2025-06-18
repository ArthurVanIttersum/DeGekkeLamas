using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    NavMeshAgent agent;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        // Touchscreen is also considered mouse button 0
        if (Input.GetMouseButtonDown(0))
        {
            // Raycast from camera position to mouse pos
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction);
            if (Physics.Raycast(ray, out RaycastHit info))
            {
                agent.SetDestination(info.point);
            }
        }
    }
}
