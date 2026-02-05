#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class AddComponentToChild : MonoBehaviour
{
    [SerializeField] private MonoScript componentToAdd;

    [ContextMenu("Add Component To All Children With Collider")]
    private void AddComponentToAllChildWithACollider()
    {
        if (componentToAdd == null)
        {
            Debug.LogError("No script assigned!");
            return;
        }

        var type = componentToAdd.GetClass();

        if (type == null || !typeof(MonoBehaviour).IsAssignableFrom(type))
        {
            Debug.LogError("Script is not a MonoBehaviour!");
            return;
        }

        int count = 0;

        foreach (Collider col in GetComponentsInChildren<Collider>(true))
        {
            if (!col.gameObject.GetComponent(type))
            {
                col.gameObject.AddComponent(type);
                count++;
            }
        }

        Debug.Log($"Added component to {count} objects.");
    }
}
#endif