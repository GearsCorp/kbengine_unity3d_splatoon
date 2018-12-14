using CBFrame.Sys;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBeginCondition : CBArtCondition {
    public TestBeginCondition(List<object> args) : base(args)
    {

    }

    public override bool CheckCondition()
    {
        Debug.Log("TestBeginCondition:CheckCondition");
        return true;
    }

    public override void OnReset()
    {
        
    }
}
