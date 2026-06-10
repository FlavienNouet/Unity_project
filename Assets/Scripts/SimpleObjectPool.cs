using System.Collections.Generic;
using UnityEngine;

public class SimpleObjectPool : MonoBehaviour
{
    public GameObject prefab;
    public int initialSize = 10;

    List<GameObject> pool = new List<GameObject>();

    void Awake()
    {
        for (int i = 0; i < initialSize; i++)
        {
            var go = Instantiate(prefab, transform);
            go.SetActive(false);
            pool.Add(go);
        }
    }

    public GameObject GetFromPool()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                return pool[i];
            }
        }

        var go = Instantiate(prefab, transform);
        go.SetActive(false);
        pool.Add(go);
        return go;
    }

    public void ReturnToPool(GameObject go)
    {
        go.SetActive(false);
        go.transform.SetParent(transform);
    }
}
