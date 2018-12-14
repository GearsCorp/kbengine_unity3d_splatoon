using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 渲染物体的ID数值标号
/// </summary>
public class IDManager : MonoBehaviour {

    public int ObjectId = 0;
    private void Awake()
    {
        ObjectId = SplatManagerSystem.instance.GameObjectID++;
     //   Debug.Log("ObjectId:::"+ ObjectId   + ",gameObject:::" +gameObject.name);
    }
}


/// <summary>
/// 人物的ID数值标号
/// </summary>
public class IDPlayerManager : MonoBehaviour
{
    public  int PlayerId = 0;
    public static int StaticPlayerId = 0;
    private void Awake()
    {
        PlayerId = StaticPlayerId++;
        //   Debug.Log("ObjectId:::"+ ObjectId   + ",gameObject:::" +gameObject.name);
    }
}
