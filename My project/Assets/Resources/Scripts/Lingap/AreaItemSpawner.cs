using UnityEngine;

[System.Serializable]
public class SpawnableItem
{
    public GameObject prefab;
    public Vector2 spawnPosition;
}

public class AreaItemSpawner : MonoBehaviour
{
    [Header("Items to Spawn in Area")]
    [SerializeField] private SpawnableItem[] itemsToSpawn;

    private void Start()
    {
        foreach (var item in itemsToSpawn)
        {
            if (item.prefab != null)
            {
                Instantiate(item.prefab, item.spawnPosition, Quaternion.identity, transform);
            }
        }
    }
}