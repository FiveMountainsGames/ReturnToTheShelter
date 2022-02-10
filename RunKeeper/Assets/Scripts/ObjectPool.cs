using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;
    [HideInInspector] public List<GameObject> pooledObjects;
    [SerializeField] public List<GameObject> objectToPoolType1;
    [SerializeField] public List<GameObject> objectToPoolType2;
    [SerializeField] public List<GameObject> objectToPoolType3;
    public Transform objectContainer;
    public int amountToPool;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        pooledObjects = new List<GameObject>();
        GameObject tmp;
        for (int j=0; j < objectToPoolType1.Count; j++)
        {
            for (int i = 0; i < amountToPool; i++)
            {
                tmp = Instantiate(objectToPoolType1[j], objectContainer);
                tmp.name = tmp.name.Replace("(Clone)", "");
                tmp.SetActive(false);
                pooledObjects.Add(tmp);
            }
        }

        for (int j = 0; j < objectToPoolType2.Count; j++)
        {
            for (int i = 0; i < amountToPool; i++)
            {
                tmp = Instantiate(objectToPoolType2[j], objectContainer);
                tmp.name = tmp.name.Replace("(Clone)", "");
                tmp.SetActive(false);
                pooledObjects.Add(tmp);
            }
        }

        for (int j = 0; j < objectToPoolType3.Count; j++)
        {
            for (int i = 0; i < amountToPool; i++)
            {
                tmp = Instantiate(objectToPoolType3[j], objectContainer);
                tmp.name = tmp.name.Replace("(Clone)", "");
                tmp.SetActive(false);
                pooledObjects.Add(tmp);
            }
        }
    }

    private GameObject GetPooledObject(GameObject requestedObj)
    {
        for(int i=0; i < pooledObjects.Count; i++)
        {
            if (requestedObj.name == pooledObjects[i].name)
            {
                if (!pooledObjects[i].activeInHierarchy)
                {
                    return pooledObjects[i];
                }
            }
        }
        return null;
    }

    public void CreateObject(GameObject obj, Vector3 position)
    {
        GameObject poolObj = GetPooledObject(obj);
        if (poolObj != null)
        {
            poolObj.transform.transform.position = position;
            poolObj.SetActive(true);
        }
    }

    public void DeleteObject(GameObject obj)
    {
        if (obj != null)
        {
            obj.SetActive(false);
        }
    }
}
