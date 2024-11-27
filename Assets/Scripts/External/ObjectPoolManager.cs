using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static List<PooledObjects> ObjectPools = new List<PooledObjects>();

    public static GameObject SpawnObject(GameObject prefab, Vector3 spawnPos, Quaternion spawnRot)
    {
        PooledObjects pool = null;

        foreach(PooledObjects p in ObjectPools)
        {
            if(p.LookUpString == prefab.name)
            {
                pool = p;
                break;
            }
        }

        if(pool == null)
        {
            pool = new PooledObjects() { LookUpString = prefab.name};
            ObjectPools.Add(pool);
        }

        GameObject spawnableObj = null;
        foreach(GameObject go in pool.InactiveObjects)
        {
            if(go != null)
            {
                spawnableObj = go;
                break;
            }
        }

        if(spawnableObj == null)
        {
            spawnableObj = Instantiate(prefab, spawnPos, spawnRot);
        }
        else
        {
            spawnableObj.transform.position = spawnPos;
            spawnableObj.transform.rotation = spawnRot;
            pool.InactiveObjects.Remove(spawnableObj);
            spawnableObj.SetActive(true);
        }

        return spawnableObj;
    }

    public static void ReturnObjectToPool(GameObject go)
    {
        string goName = go.name.Substring(0, go.name.Length - 7); //remove the word (clone) from the obj

        PooledObjects pool = null;
        foreach(PooledObjects p in ObjectPools)
        {
            if(p.LookUpString == goName)
            {
                pool = p;
            }
        }
        if(pool == null)
        {
            return;
        }
        else
        {
            go.SetActive(false);
            pool.InactiveObjects.Add(go);
        }
    }
}

public class PooledObjects
{
    public string LookUpString;
    public List<GameObject> InactiveObjects = new List<GameObject>();
}
