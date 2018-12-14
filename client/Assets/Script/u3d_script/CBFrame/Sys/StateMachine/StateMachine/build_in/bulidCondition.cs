using CBFrame.Sys;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class bulidCondition : SMCondition
{
    public bulidCondition(List<string> args, GameObject obj) : base(args, obj)
    {

    }
    public override bool Condititon()
    {
        return ifTriggerConditions();
    }
}


