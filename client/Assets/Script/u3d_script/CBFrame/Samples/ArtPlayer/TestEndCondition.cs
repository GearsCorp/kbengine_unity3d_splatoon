using CBFrame.Sys;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEndCondition : CBArtCondition
{
    private float _duration;

    public TestEndCondition(List<object> args) : base(args)
    {

    }

    public override bool CheckCondition()
    {
        _duration += Time.deltaTime;

        if(_duration > 3f)
        {
            Debug.Log("TestEndCondition:CheckCondition");
        }

        return _duration > 3f;
    }

    public override void OnReset()
    {
        _duration = 0f;
    }
}
