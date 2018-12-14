using CBFrame.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoEventTest2 : CBComponent {

	// Use this for initialization
	void Start () {
        AddEventListener(1, Test);
	}
	
	public void Test()
    {
        Debug.Log("GoEventTest2 test func called");
    }
}
