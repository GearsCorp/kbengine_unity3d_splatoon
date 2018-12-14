using CBFrame.Sys;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STATE_IDLE : SMState
{
    public STATE_IDLE(string name, GameObject go) : base(name, go)
    {
        Debug.Log(gameObject.name);
    }

    public override void OnUpdate()
    {
        Debug.Log("STATE_IDLE OnUpdate");
    }
}