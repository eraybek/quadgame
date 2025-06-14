using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Pool_Ahmet
{
    public string tag;
    public GameObject prefab;
    public int size;
}

public class ObjectPooler_Ahmet : MonoBehaviour
{
    public static ObjectPooler_Ahmet Instance;

    public List<Pool_Ahmet> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool_Ahmet pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return null;
        }

        GameObject obj = poolDictionary[tag].Dequeue();

        obj.SetActive(true);
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        poolDictionary[tag].Enqueue(obj); // tekrar sıraya koy

        return obj;
    }

    public int GetActiveCount(string tag)
    {
        if (poolDictionary.ContainsKey(tag))
        {
            int count = 0;
            foreach (var obj in poolDictionary[tag])
            {
                if (obj != null)
                    if (obj.activeInHierarchy)
                        count++;
            }
            return count;
        }
        return 0;
    }
}
