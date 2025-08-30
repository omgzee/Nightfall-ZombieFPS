using System.Collections.Generic;
using UnityEngine;

public class ZombiePool : MonoBehaviour
{
    [SerializeField] private GameObject[] zombiePrefabs;
    [SerializeField] private int poolSize = 30;

    private List<GameObject> pooledZombies = new List<GameObject>();

    private void Awake()
    {
        foreach (GameObject prefab in zombiePrefabs)
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject zombie = Instantiate(prefab);
                zombie.SetActive(false);
                pooledZombies.Add(zombie);
            }
        }

        // SHUFFLE the list to mix different types
        for (int i = 0; i < pooledZombies.Count; i++)
        {
            var temp = pooledZombies[i];
            int randomIndex = Random.Range(i, pooledZombies.Count);
            pooledZombies[i] = pooledZombies[randomIndex];
            pooledZombies[randomIndex] = temp;
        }
    }


    public GameObject GetZombie()
    {
        List<GameObject> inactiveZombies = new List<GameObject>();

        foreach (GameObject zombie in pooledZombies)
        {
            if (zombie != null && !zombie.activeInHierarchy)
                inactiveZombies.Add(zombie);
        }

        if (inactiveZombies.Count > 0)
            return inactiveZombies[Random.Range(0, inactiveZombies.Count)];

        return null;
    }

}

