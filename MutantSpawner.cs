using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutantSpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private Transform[] mutantSpawnPoints;

    [Header("Spawn Settings")]
    [SerializeField] private float mutantSpawnInterval = 10f;
    [SerializeField] private int maxMutants = 10;
    [SerializeField] private float minDistanceFromPlayer = 5f;

    [Header("References")]
    [SerializeField] private Transform player;
    public MutantPool mutantPool; // Reference to the pool script

    private int spawnedMutantCount = 0;
    private Coroutine spawnRoutine;

    public void SpawnWave(int numberOfMutants, float interval)
    {
        if (mutantPool == null)
        {
            Debug.LogError("MutantSpawner: mutantPool is not assigned!");
            return;
        }
        if (mutantSpawnPoints == null || mutantSpawnPoints.Length == 0)
        {
            Debug.LogError("MutantSpawner: No spawn points assigned!");
            return;
        }
        if (player == null)
        {
            Debug.LogError("MutantSpawner: Player reference is not assigned!");
            return;
        }

        maxMutants = numberOfMutants;
        mutantSpawnInterval = interval;
        spawnedMutantCount = 0;

        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);

        spawnRoutine = StartCoroutine(SpawnMutantsRoutine());

        Debug.Log($"MutantSpawner: Starting to spawn {maxMutants} mutants every {mutantSpawnInterval} seconds.");
    }

    private IEnumerator SpawnMutantsRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(mutantSpawnInterval);

        while (spawnedMutantCount < maxMutants)
        {
            TrySpawnMutant();
            spawnedMutantCount++;
            yield return wait;
        }

        Debug.Log("MutantSpawner: Finished spawning mutants.");
        spawnRoutine = null;
    }

    private void TrySpawnMutant()
    {
        if (mutantSpawnPoints.Length == 0 || mutantPool == null || player == null) return;

        Transform spawnPoint = GetSafeSpawnPoint();
        if (spawnPoint == null)
        {
            Debug.LogWarning("MutantSpawner: No safe spawn point found to spawn mutant!");
            return;
        }

        GameObject mutant = mutantPool.GetMutant();
        mutant.transform.position = spawnPoint.position;
        mutant.transform.rotation = spawnPoint.rotation;
        Debug.Log($"MutantSpawner: Spawned mutant from pool at {spawnPoint.position}");
    }

    private Transform GetSafeSpawnPoint()
    {
        List<Transform> safePoints = new List<Transform>();

        foreach (var point in mutantSpawnPoints)
        {
            if (Vector3.Distance(point.position, player.position) > minDistanceFromPlayer)
                safePoints.Add(point);
        }

        if (safePoints.Count == 0)
            return null;

        return safePoints[Random.Range(0, safePoints.Count)];
    }
}
