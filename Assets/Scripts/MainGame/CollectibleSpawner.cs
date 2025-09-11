using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    private int totalCollectibles;          // Total number to spawn
    [SerializeField] private GameObject[] collectiblePrefabs;     // Array of collectible prefabs
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(10, 10); // Area (XZ) where items spawn
    [SerializeField] private float spawnHeight = 0f;              // Height above plane

    [Header("Collision Settings")]
    [SerializeField] private float checkRadius = 0.5f;            // Clearance needed around collectible
    [SerializeField] private float maxSearchDistance = 5f;        // Max distance to search for free space
    [SerializeField] private float searchStep = 0.5f;             // Step size when searching outward

    [Header("References")]
    [SerializeField] private Transform plane;

    void Start()
    {
        totalCollectibles = GameManager.Instance.GetTotalCollectible();
        SpawnCollectibles();
    }

    void SpawnCollectibles()
    {
        if (collectiblePrefabs == null || collectiblePrefabs.Length == 0)
        {
            Debug.LogWarning("No collectible prefabs assigned!");
            return;
        }

        int spawned = 0;

        while (spawned < totalCollectibles)
        {
            // Pick a random prefab
            GameObject prefab = collectiblePrefabs[Random.Range(0, collectiblePrefabs.Length)];

            // Get random position
            Vector3 pos = GetRandomPosition();

            // Find nearest free position (avoids walls / overlaps)
            Vector3 freePos = FindNearestFreePosition(pos, checkRadius, maxSearchDistance, searchStep);

            // If position is blocked even after search, skip
            if (Physics.CheckSphere(freePos, checkRadius))
            {
                Debug.LogWarning("Could not find valid spot for collectible " + (spawned + 1));
                break;
            }

            // Spawn collectible
            freePos.y = 0f;
            GameObject collectible = Instantiate(prefab, freePos, Quaternion.identity, transform);
            spawned++;
        }
    }

    Vector3 GetRandomPosition()
    {
        float x = Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f);
        float z = Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f);
        float y = spawnHeight;

        Vector3 pos = new Vector3(x, y, z);

        if (plane != null)
            pos += plane.position;

        return pos;
    }

    Vector3 FindNearestFreePosition(Vector3 startPos, float radius, float maxDist, float step)
    {
        // If start is already free, use it
        if (!Physics.CheckSphere(startPos, radius))
            return startPos;

        // Expand outward in rings
        for (float r = step; r <= maxDist; r += step)
        {
            int steps = Mathf.CeilToInt(2 * Mathf.PI * r / step); // points around circle
            for (int i = 0; i < steps; i++)
            {
                float angle = (360f / steps) * i * Mathf.Deg2Rad;
                Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * r;
                Vector3 candidate = startPos + offset;
                int layerMask = ~LayerMask.GetMask("Ground"); // ignore "Ground" layer
                if (!Physics.CheckSphere(candidate, radius, layerMask))
                    return candidate;
            }
        }

        // If no free spot found, return original (likely blocked)
        return startPos;
    }

    public int TotalCollectible()
    {
        return totalCollectibles;
    }
}
