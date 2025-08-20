using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    [Header("平台预制体")]
    public GameObject platformPrefab;

    [Header("生成设置")]
    public Vector3 startPosition = new Vector3(0f, 0.4f, 5f);
    public float platformSpacing = 1f;
    public int initialCount = 50;
    public int threshold = 40;
    public int replenishAmount = 10;

    private Queue<GameObject> platforms = new Queue<GameObject>();
    private Vector3 currentSpawnPosition;

    void Start()
    {
        currentSpawnPosition = startPosition;
        SpawnPlatforms(initialCount);
    }

    void Update()
    {
        if (platforms.Count < threshold)
        {
            SpawnPlatforms(replenishAmount);
        }
    }

    void SpawnPlatforms(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject platform = Instantiate(platformPrefab, currentSpawnPosition, Quaternion.identity);
            platforms.Enqueue(platform);
            currentSpawnPosition += new Vector3(0f, 0f, platformSpacing);
        }
    }
}
