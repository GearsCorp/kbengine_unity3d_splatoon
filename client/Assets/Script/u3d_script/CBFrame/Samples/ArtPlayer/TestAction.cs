using System.Collections;
using System.Collections.Generic;
using CBFrame.Sys;
using UnityEngine;

public class TestAction : CBArtAction
{
    public TestAction(List<object> args, GameObject obj): base(args, obj)
    {

    }

    protected override void OnBegin()
    {
        Debug.Log("TestAction:OnBegin");
    }

    protected override void OnEnd()
    {
        Debug.Log("TestAction:OnEnd");
    }

    protected override void OnReset()
    {
        
    }

    protected override ArtActionState OnUpdate()
    {
        Debug.Log("TestAction:OnUpdate");
        return ArtActionState.AAS_SUCCEED;
    }
}
