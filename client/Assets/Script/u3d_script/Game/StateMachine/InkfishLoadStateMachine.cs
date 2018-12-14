using CBFrame.Sys;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkfishLoadStateMachine : MonoBehaviour
{
    public CBStateMachine sm;
    // Use this for initialization
    void Start()
    {
        sm = gameObject.GetComponent<CBStateMachine>();
        sm.LoadFromFile("data/avatar_state_machine"); 
    }
     
    // Update is called once per frames
    void Update()
    {

    }
}