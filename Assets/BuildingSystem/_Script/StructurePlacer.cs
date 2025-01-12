using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using DG.Tweening;

public class StructurePlacer : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> placedObjects = new List<GameObject>();

    [SerializeField]
    private float scalingDelay = 0.3f, destroyDelay = 0.1f;

    [SerializeField]
    private float heightOffset = 0f; // Huidige hoogte-offset
    [SerializeField]
    private float heightStep = 2f;  // Stapgrootte om hoogte te veranderen

    private void OnEnable()
    {
        Debug.Log("StructurePlacer enabled");

        // Ensure placedObjects list is correctly initialized
        if (placedObjects == null)
        {
            placedObjects = new List<GameObject>();
        }
    }

    private void OnDisable()
    {
        Debug.Log("StructurePlacer disabled");

        // Stop animations for all placed objects to prevent lingering DOTween effects
        foreach (GameObject obj in placedObjects)
        {
            if (obj != null)
            {
                obj.transform.DOKill();
            }
        }
    }

    private int GetFreeIndex()
    {
        int indexOfNull = placedObjects.IndexOf(null);
        if (indexOfNull > -1)
        {
            return indexOfNull;
        }
        placedObjects.Add(null);
        return placedObjects.Count - 1;
    }

    public Quaternion GetObjectsRotation(int index)
    {
        if (index >= 0 && index < placedObjects.Count && placedObjects[index] != null)
        {
            return placedObjects[index].transform.GetChild(0).rotation;
        }
        return Quaternion.identity;
    }

    public int PlaceStructure(GameObject objectToPlace, Vector3 position, Quaternion rotation, int gridIndex)
    {
        int freeIndex = GetFreeIndex();
        GameObject newObject = Instantiate(objectToPlace);

        // Selecteer de juiste parent op basis van de gridIndex
        Transform parentTransform;
        switch (gridIndex)
        {
            case 0:
                parentTransform = transform.Find("Child_1");
                break;
            case 1:
                parentTransform = transform.Find("Child_2");
                break;
            case 2:
                parentTransform = transform.Find("Child_3");
                break;
            default:
                parentTransform = transform; // Fallback naar de root
                break;
        }

        newObject.transform.SetParent(parentTransform);

        // Gebruik heightOffset voor de Y-positie
        Vector3 placementPosition = new Vector3(position.x, heightOffset, position.z);
        newObject.transform.position = placementPosition;
        newObject.transform.GetChild(0).rotation = rotation;
        newObject.transform.localScale = new Vector3(1, 0, 1);

        placedObjects[freeIndex] = newObject;
        newObject.transform.DOScaleY(1, scalingDelay);
        return freeIndex;
    }


    public void RemoveObjectAt(int index)
    {
        if (index >= 0 && index < placedObjects.Count && placedObjects[index] != null)
        {
            GameObject objToRemove = placedObjects[index];
            objToRemove.transform.DOKill();
            objToRemove.transform.DOScaleY(0, destroyDelay).OnComplete(() => Destroy(objToRemove));
            placedObjects[index] = null;
        }
    }

    public void IncreaseHeight(float step)
    {
        heightOffset += step;
        Debug.Log($"Height increased: {heightOffset}");
    }

    public void DecreaseHeight(float step)
    {
        heightOffset -= step;
        Debug.Log($"Height decreased: {heightOffset}");
    }



}
