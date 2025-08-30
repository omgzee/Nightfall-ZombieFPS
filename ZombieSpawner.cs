using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private bool loopSpawning = true;
    [SerializeField] private int maxZombies = 50;
    [SerializeField] private float minDistanceFromPlayer = 5f;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private ZombiePool zombiePool;

    private int spawnedCount = 0;

    private void Start()
    {
        if (loopSpawning)
            StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(spawnInterval);
        while (spawnedCount < maxZombies)
        {
            TrySpawnZombie();
            spawnedCount++;
            yield return wait;
        }
    }

    private void TrySpawnZombie()
    {
        if (spawnPoints.Length == 0 || zombiePool == null || player == null) return;

        Transform spawnPoint = GetSafeSpawnPoint();
        if (spawnPoint == null) return;

        GameObject zombie = zombiePool.GetZombie();
        if (zombie != null)
        {
            zombie.transform.position = spawnPoint.position;
            zombie.transform.rotation = spawnPoint.rotation;
            zombie.SetActive(true);
        }
    }

    private Transform GetSafeSpawnPoint()
    {
        List<Transform> safePoints = new List<Transform>();

        foreach (var point in spawnPoints)
        {
            if (Vector3.Distance(point.position, player.position) > minDistanceFromPlayer)
                safePoints.Add(point);
        }

        if (safePoints.Count == 0) return null;

        return safePoints[Random.Range(0, safePoints.Count)];
    }
}
