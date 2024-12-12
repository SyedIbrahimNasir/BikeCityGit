using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetBannerReplacer : MonoBehaviour

{
    [System.Serializable]
    public class ReplacementGroup
    {
        public GameObject[] objectsToReplace; // Objects to replace
        public GameObject[] replacementPrefabs; // List of possible prefabs
    }

    [Header("Replacement Groups")]
    public ReplacementGroup[] replacementGroups;

    [ContextMenu("Replace Objects")]
    public void ReplaceObjects()
    {
        if (replacementGroups == null || replacementGroups.Length == 0)
        {
            Debug.LogWarning("No replacement groups defined.");
            return;
        }

        foreach (ReplacementGroup group in replacementGroups)
        {
            if (group.replacementPrefabs == null || group.replacementPrefabs.Length == 0)
            {
                Debug.LogError("A replacement group has no assigned prefabs.");
                continue;
            }

            if (group.objectsToReplace == null || group.objectsToReplace.Length == 0)
            {
                Debug.LogWarning("No objects assigned to replace in a replacement group.");
                continue;
            }

            foreach (GameObject obj in group.objectsToReplace)
            {
                if (obj != null)
                {
                    // Store the position and rotation of the object
                    Vector3 position = obj.transform.position;
                    Quaternion rotation = obj.transform.rotation;

                    // Select a random prefab from the list
                    GameObject randomPrefab = group.replacementPrefabs[Random.Range(0, group.replacementPrefabs.Length)];

                    // Instantiate the selected prefab
                    GameObject newObject = Instantiate(randomPrefab, position, rotation);

                    // Ensure the new object's scale is 1
                    newObject.transform.localScale = Vector3.one;

                    // Destroy the original object
                    DestroyImmediate(obj);
                }
            }
        }

        Debug.Log("Replacement with multiple prefabs completed.");
    }
}

