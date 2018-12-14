using CBFrame.Sys;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class compare_velocity : SMCondition
{
    public compare_velocity(List<string> args, GameObject obj) : base(args, obj)
    {

    }
    public override bool Condititon()
    {
       
        return ifTriggerConditions();
    }
}
