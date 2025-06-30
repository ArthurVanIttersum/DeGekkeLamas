using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerAnimations : MonoBehaviour
{
    private NavMeshAgent agent;
    public Sprite forward;
    public Sprite walkLeft;
    public Sprite walkRight;
    private Image theImage;
    private Quaternion rotation;
    private void Start()
    {
        agent = GetComponentInParent<NavMeshAgent>();
        theImage = GetComponentInChildren<Image>();
        rotation = Camera.main.transform.rotation;
    }
    // Update is called once per frame
    void Update()
    {
        transform.rotation = rotation;
        Vector3 velocity = agent.velocity;
        if (Mathf.Abs(velocity.z) > Mathf.Abs(velocity.x))
        {
            if (velocity.z > 0)
            {
                //right
                theImage.sprite = walkRight;
            }
            else
            {
                //left
                theImage.sprite = walkLeft;
            }
        }
        else
        {
            //forward
            theImage.sprite = forward;
        }

    }
}
