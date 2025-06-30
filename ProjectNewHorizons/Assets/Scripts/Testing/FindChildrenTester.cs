using UnityEngine;

public class FindChildrenTester : MonoBehaviour
{
    public GameObject toTest;
    [NaughtyAttributes.Button]
    void Test()
    {
        var list = MathTools.GetAllChildren(toTest);

        foreach (var child in list)
        {
            print(child.name);
        }
    }
}
