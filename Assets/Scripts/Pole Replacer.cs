using UnityEngine;

public class PoleReplacer : MonoBehaviour
{
    [System.Serializable]
    public class ReplacementPair
    {
        public GameObject[] objectsToReplace;
        public GameObject replacementPrefab;
    }

    [Header("Replacement Pairs")]
    public ReplacementPair[] replacementPairs;

    [ContextMenu("Replace Objects")]
    public void ReplaceObjects()
    {
        if (replacementPairs == null || replacementPairs.Length == 0)
        {
            Debug.LogWarning("No replacement pairs defined.");
            return;
        }

        foreach (ReplacementPair pair in replacementPairs)
        {
            if (pair.replacementPrefab == null)
            {
                Debug.LogError("A replacement prefab is not assigned in one of the pairs.");
                continue;
            }

            if (pair.objectsToReplace == null || pair.objectsToReplace.Length == 0)
            {
                Debug.LogWarning("No objects assigned to replace in one of the pairs.");
                continue;
            }

            foreach (GameObject obj in pair.objectsToReplace)
            {
                if (obj != null)
                {
                    // Store the position and rotation of the object
                    Vector3 position = obj.transform.position;
                    Quaternion rotation = obj.transform.rotation;

                    // Instantiate the replacement prefab
                    GameObject newObject = Instantiate(pair.replacementPrefab, position, rotation);

                    // Ensure the new object's scale is 1
                    newObject.transform.localScale = Vector3.one;

                    // Destroy the original object
                    DestroyImmediate(obj);
                }
            }
        }

        Debug.Log("Replacement completed.");
    }
}


