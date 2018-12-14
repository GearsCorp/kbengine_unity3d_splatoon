using CBFrame.Sys;
using GameLogic.Configs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffect : CBArtAction
{

    private GameObject EffObject;
    private ParticleSystem _effObj = null;
    private Transform _playerChildObj;

    public PlayerEffect(List<object> args, GameObject obj) : base(args, obj)
    {
        _playerChildObj = Go.transform.Find(PlayerCommonName.ChildName);
    }
    protected override void OnBegin()
    {
        if (EffObject == null)
        {
            string name = GetEffectName();
            Debug.Log("fishDiveEffect::OnBegin::" + name);
            if (name == "") return;

            EffObject = (GameObject)Resources.Load(name);
            if (EffObject == null) return;
            EffObject = MonoBehaviour.Instantiate(EffObject, _playerChildObj.position, _playerChildObj.rotation);
            _effObj = EffObject.GetComponentInChildren<ParticleSystem>();
            Debug.Log("Effect:OnUpdate:" + "name::" + name);
        }
    }

    protected override void OnEnd()
    {
        if (_effObj)
        {
            MonoBehaviour.Destroy(EffObject);
            EffObject = null;
            _effObj = null;
            Debug.Log("Effect:::_OnEnd!!!!");
        }
    }

    protected override void OnReset()
    {
    }

    protected override ArtActionState OnUpdate()
    {
        if (_effObj == null)
            return ArtActionState.ASS_FAILURE;

        EffObject.transform.position = _playerChildObj.position;
        EffObject.transform.rotation = _playerChildObj.rotation;
      //  Debug.Log("DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD::"+ _playerChildObj.name+ ",transform.positio::"+ _effObj.transform.position);
        if (_effObj.isPlaying)
            return ArtActionState.AAS_RUNNING;
        else if (_effObj.isStopped)
        {
            return ArtActionState.AAS_END;
        }
           
        else
            return ArtActionState.AAS_SUCCEED;
    }

    public string GetEffectName()
    {
        if (_args.Count == 0) return "";
        return Convert.ToString(_args[0]);
    }


    //设置
    public virtual void SetGameObject(Transform obj)
    {
        _playerChildObj = obj;
    }

}
