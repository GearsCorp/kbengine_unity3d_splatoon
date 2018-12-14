using CBFrame.Sys;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatrtForm : MonoBehaviour {

    // Use this for initialization


    public CBStateMachine sm;

    public GameObject dataObj;

    int num = 1;
    void Start()
    {

        //   sm.LoadFromFile("data/avatar_state_machine");
        GetComponent<Button>().onClick.AddListener(OnClick);

        Invoke("numnum", 3);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnClick()
    {
        if (sm)
        {
            if (num == 1) num = 2;
            else num = 1;
            sm.SetParameter("AvatarForm", num);
            Debug.Log("avatarForm::" + num);
        }
    }

    void numnum()
    {
        sm = dataObj.GetComponent<CBStateMachine>();
    }
}
