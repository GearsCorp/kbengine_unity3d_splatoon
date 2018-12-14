using CBFrame.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventFire : CBComponent {
    public bool Fire;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Fire)
        {
            TriggerEvent(1);
            Fire = false;
        }
	}
}
