using System.Collections.Generic;
using UnityEngine;

public class MutantPool : MonoBehaviour
{
    [SerializeField] private GameObject mutantPrefab;
    [SerializeField] private int initialPoolSize = 10;

    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject obj = Instantiate(mutantPrefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetMutant()
    {
        if (pool.Count > 0)
        {
            GameObject mutant = pool.Dequeue();
            mutant.SetActive(true);
            return mutant;
        }
        else
        {
            // Optionally expand pool if empty
            GameObject mutant = Instantiate(mutantPrefab);
            return mutant;
        }
    }

    public void ReturnMutant(GameObject mutant)
    {
        mutant.SetActive(false);
        mutant.transform.position = Vector3.zero;
        mutant.transform.rotation = Quaternion.identity;

        // Optional: reset navmesh or physics components
        var rb = mutant.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        pool.Enqueue(mutant);
    }

}
