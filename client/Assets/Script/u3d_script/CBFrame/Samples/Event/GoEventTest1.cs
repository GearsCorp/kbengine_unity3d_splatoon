using CBFrame.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoEventTest1 : CBComponent
{

    // Use this for initialization
    void Start()
    {
        AddEventListener(1, Test);
    }

    public void Test()
    {
        Debug.Log("GoEventTest1 test func called");
    }
}
