using CBFrame.Sys;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUMAN_IDLE : SMState
{
    private Avatar _avatar;
    private CharacterMotion _characterMotionObj;
    public HUMAN_IDLE(string name, GameObject go) :
        base(name, go)
    {
        Debug.Log("gameObject::" + gameObject.name);
        _avatar = gameObject.GetComponent<Avatar>();
        _characterMotionObj = gameObject.transform.GetComponent<CharacterMotion>();
       // Debug.Log("_characterMotionObj::" + gameObject.name);
    }

    public override void OnUpdate()
    {
        _characterMotionObj.Idle();
    }

}
