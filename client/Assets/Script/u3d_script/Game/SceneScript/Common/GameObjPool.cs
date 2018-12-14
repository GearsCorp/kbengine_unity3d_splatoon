using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjPool : MonoBehaviour
{
   // private Dictionary<string, List<GameObject>> pools = new Dictionary<string, List<GameObject>>();

    private List<GameObject> pools = new List<GameObject>();

    private static GameObjPool instance;

    public static GameObjPool GetInstance()
    {
        if (instance == null)
        {
            instance = new GameObject("GameObjPool").AddComponent<GameObjPool>();
        }
        return instance;
    }

    private int num = 100;
    public GameObject InstantiateObject(GameObject NameObj, Vector3 Position, Quaternion Rotation)
    {
        //Debug.Log("pools:::" + NameObj.name);
        //if (!pools.ContainsKey(NameObj.name) || pools[NameObj.name].Count == 0)
        //{
        //    GameObject obj = Instantiate(NameObj, Position, Rotation) as GameObject;
        //    obj.name = NameObj.name;
        //    return obj;
        //}
        //else
        //{
        //    GameObject obj = pools[NameObj.name][0];
        //    obj.SetActive(true);
        //    pools[NameObj.name].Remove(obj);
        //    return obj;
        //}

        if ( pools.Count == 0)
        {
            GameObject obj = Instantiate(NameObj, Position, Rotation) as GameObject;
            obj.name = "buyllet_" + num.ToString();
            num++;
          //  Debug.Log("dddddddddddddddd:::create bullet::::" + obj.name);
            return obj;
        }
        else
        {
            GameObject obj = pools[0];
            obj.transform.position = Position;
            obj.transform.rotation = Rotation;
            obj.SetActive(true);
            pools.Remove(obj);
           // Debug.Log("getgetgetgetgetgetgetget:::create bullet::::" + obj.name);
            return obj;
        }
    }

    public GameObject InstantiateObject(GameObject NameObj)
    {
        return InstantiateObject(NameObj, Vector3.zero, Quaternion.identity);
    }

    public void DestroyObject(GameObject NameObj)
    {
        //Debug.Log("pools:::"+ NameObj.name);
        //NameObj.SetActive(false);
        //if (!pools.ContainsKey(NameObj.name))
        //{
        //    pools[NameObj.name] = new List<GameObject>();
        //}
        //pools[NameObj.name].Add(NameObj); 
        NameObj.SetActive(false);
        pools.Add(NameObj);
       // Debug.Log("pools:::" + pools.Count + ",data:::" + NameObj.name);
    }

    public void OnDestroy()
    {
        for(int i = 0; i < pools.Count; i++)
        {
            Destroy(pools[i]);
            pools[i] = null;
        }
        Resources.UnloadUnusedAssets();
    }
}

