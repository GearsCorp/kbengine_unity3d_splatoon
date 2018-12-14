using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CBFrame.Sys;


public class STATE_MOVE : SMState
{
    int i = 0;
    public STATE_MOVE(string name, GameObject go) : base(name, go)
    {
        Debug.Log(gameObject.name);
    }

    public override void OnUpdate()
    {
        i++;
        if ((i % 10) == 0) {
            gameObject.transform.Translate(gameObject.transform.forward * 2f);
        } 
        
    }
}
