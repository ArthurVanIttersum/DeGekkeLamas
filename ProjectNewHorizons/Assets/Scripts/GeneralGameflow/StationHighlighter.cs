using System.Collections.Generic;
using UnityEngine;

public class StationHighlighter : MonoBehaviour
{
    public GridActivator[] stations;
    public Material highlightedMaterial;
    List<GameObject> highlitObjects = new();
    //Transform container;

    public static StationHighlighter instance;

    void Awake()
    {
        instance = this;
        //container = new GameObject("HighlightContainer").transform;
    }

    GridActivator FindStationByType(DishType type)
    {
        foreach (GridActivator station in stations)
        {
            if (station.stationType == type) return station;
        }
        return null;
    }

    public void Highlight(DishType type)
    {
        GridActivator station = FindStationByType(type);

        List<GameObject> toAffect = MathTools.GetAllChildren(station.physicalObject);
        foreach(GameObject obj in toAffect)
        {
            if (obj.TryGetComponent(out MeshRenderer renderer)) 
            {
                renderer = Instantiate(renderer, obj.transform);
                renderer.sharedMaterial = highlightedMaterial;
                renderer.transform.localPosition = Vector3.zero;
                renderer.transform.localScale = Vector3.one;
                renderer.transform.localRotation = Quaternion.identity;
                highlitObjects.Add(renderer.gameObject);
            }
        }
    }

    public void RemoveHighlight()
    {
        Debug.Log($"Removed {highlitObjects.Count} highlights");
        for (int i = highlitObjects.Count; i > 0; i--)
        {
            highlitObjects[i-1].transform.parent = null;
            Destroy(highlitObjects[i-1]);
            highlitObjects.RemoveAt(i-1);
        }
    }
}
