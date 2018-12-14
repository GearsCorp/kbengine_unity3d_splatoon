using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CBFrame.Sys;
using UnityEngine.UI;

public class Test_machineTest : MonoBehaviour {
    // Use this for initialization

   // CBStateMachine sm;
    public bool moveFlag = false;

  //  public bool testMoveFlag;

    public CBStateMachine sm;

    public GameObject dataObj;
    void Start() {
       // testMoveFlag = moveFlag;

     //   sm.LoadFromFile("data/avatar_state_machine");
        GetComponent<Button>().onClick.AddListener(OnClick);

        Invoke("numnum", 3);
    }

    // Update is called once per frame
    void Update() {
     
    }

    void OnClick()
    {
        if (sm)
        {
            moveFlag = !moveFlag;
            sm.SetParameter("IsMove", moveFlag);
        }
    }

    void numnum()
    {
        sm = dataObj.GetComponent<CBStateMachine>();
    }
}
