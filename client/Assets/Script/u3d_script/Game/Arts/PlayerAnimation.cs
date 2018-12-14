using CBFrame.Sys;
using GameLogic.Configs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : CBArtAction
{
    private Animation _animationObj;
    private string _animationName = "";
    private Transform _playerChildObj;

    public PlayerAnimation(List<object> args, GameObject obj) : base(args,obj)
    {
        _playerChildObj = Go.transform.Find(PlayerCommonName.ChildName);
        _animationObj   = _playerChildObj.Find(PlayerCommonName.PersonModelName).GetComponent<Animation>();
    }

    protected override void OnBegin()
    {
        _animationName = GetAnimatorPath();
      //  Debug.Log("PlayerAnimation:OnBegin:" + "name::" + _animationName);
        if (_animationName == "" || (_animationObj.IsPlaying(_animationName)))
            return;
           
        //if (_animationName != "ava_dive")
        //    _animationObj.CrossFade(_animationName);
        //else
        //{
            _animationObj.Play(_animationName);
      //  }
        //   Debug.Log("PlayerAnimation:OnBegin");
    }

    protected override void OnEnd()
    {
        if (_animationObj && _animationName != "")
        {
         //   Debug.Log("PlayerAnimation:OnEnd:" + "name::" + _animationName);
            _animationObj.Stop(_animationName);
         //   Debug.Log("animation_stop::" + _animationName);
         //   Debug.Log("_animationObj.clip.name == _animationName:::" + _animationObj.clip.name + "," + _animationName);
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


        //_animationName = getAnimatorPath();

        //if (_animationName == "" || (_animationObj.IsPlaying(_animationName)))
        //    return ArtActionState.AAS_SUCCEED;

        //Debug.Log("PlayerAnimation:OnUpdate:" +"name::" + _animationName);
        //_animationObj.CrossFade(_animationName);
        //return ArtActionState.AAS_SUCCEED;
    }


    public string GetAnimatorPath()
    {
        if (_args.Count == 0) return "";

        return Convert.ToString(_args[0]);
    }

    private void chooseAnimation()
    {
    }
}
