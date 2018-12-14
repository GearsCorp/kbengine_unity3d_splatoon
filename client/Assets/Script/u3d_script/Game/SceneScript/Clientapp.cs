using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clientapp : KBEMain {

    static Clientapp _instance;
    // Use this for initialization
    public static Clientapp Instance
    {
        get { return _instance; }
    }
    public override void installEvents()
    {
        base.installEvents();
    }

    public override void initKBEngine()
    {
        base.initKBEngine();
    }

    void OnApplicationQuit()
    {
       // KBEngine.Event.fireIn("logout");
        //Debug.Log("dddddddddddddddddddddddddddddddddddddddddd");
    }
}
