using UnityEngine;

public class InvertActive : MonoBehaviour
{
    public void ToggleActive()
    {
        this.gameObject.SetActive(this.gameObject.activeSelf);
    }
}
