using CBFrame.Sys;
using CBFrame.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompareInt : SMCondition
{
    public override bool Condititon()
    {
        throw new System.NotImplementedException();
    }
}

public class Character : CBComponent {

    private CBStateMachine _sm;

    private void Awake()
    {
        //EventDispatch.RegEvent("AVATAR_COMBAT", OnCombat);
        //EventDispatch.RegEvent("AVATAR_MOVE", OnMove);
    }

    public void OnCombat()
    {
        
        _sm.ChangeState("STATE_COMBAT", .0f);
    }

    public void OnMove()
    {

        _sm.ChangeState("STATE_MOVE", .0f);
    }
}
