using CBFrame.Sys;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loadMachine : MonoBehaviour {
    public CBStateMachine sm;
    // Use this for initialization
    void Start () {
        sm = gameObject.GetComponent<CBStateMachine>();
        sm.LoadFromFile("sample/data/sample_state_machine");
        sm.UpdateState();
    }
	
	// Update is called once per frames
	void Update () {
		
	}
}
