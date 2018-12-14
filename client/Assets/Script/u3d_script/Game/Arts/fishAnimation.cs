using CBFrame.Sys;
using GameLogic.Configs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fishAnimation : CBArtAction
{
    private Animation _animationObj;
    private string _animationName = "";
    private Transform _playerChildObj;

    public fishAnimation(List<object> args, GameObject obj) : base(args, obj)
    {
        _playerChildObj = Go.transform.Find(PlayerCommonName.ChildName);
        _animationObj = _playerChildObj.Find(PlayerCommonName.FishModelName).GetComponent<Animation>();
    }

    ~fishAnimation()
    {
        _animationObj = null;
        _playerChildObj = null;
    }
    protected override void OnBegin()
    {
        _animationName = GetAnimatorName();
        Debug.Log("PlayerAnimation:OnBegin:" + "name::" + _animationName);
        if (_animationName == "" || (_animationObj.IsPlaying(_animationName)))
            return;

        _animationObj.CrossFade(_animationName);
    }

    protected override void OnEnd()
    {
        if (_animationObj && _animationName != "")
        {
            Debug.Log("PlayerAnimation:OnEnd:" + "name::" + _animationName);
            _animationObj.Stop(_animationName);
        }
    }

    protected override void OnReset()
    {
        _animationName = "";
    }

    protected override ArtActionState OnUpdate()
    {
        if (_animationObj.IsPlaying(_animationName))
            return ArtActionState.AAS_RUNNING;
        else if (!_animationObj.IsPlaying(_animationName))
            return ArtActionState.AAS_END;
        else
            return ArtActionState.AAS_SUCCEED;
    }

    public string GetAnimatorName()
    {
        if (_args.Count == 0) return "";
        return Convert.ToString(_args[0]);
    }

    private void chooseAnimation()
    {
    }

    
}
