using CBFrame.Sys;
using GameLogic.Configs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fishDiveEffect : CBArtAction
{
    private GameObject EffObject;
    private ParticleSystem _effObj = null;
    private ParticleSystem[] particleSystems;
    private ParticleSystem particleSystem;
    private Transform _playerChildObj;

    public fishDiveEffect(List<object> args, GameObject obj) : base(args, obj)
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
            _effObj = EffObject.GetComponent<ParticleSystem>();
            Debug.Log("fishDiveEffect:OnUpdate:" + "name::" + name);
        }
    }

    protected override void OnEnd()
    {
        if (_effObj)
        {
            MonoBehaviour.Destroy(EffObject);
            EffObject = null;
            _effObj = null;
            Debug.Log("fishDiveEffect:::_OnEnd!!!!");
        }
    }

    protected override void OnReset()
    {
    }

    protected override ArtActionState OnUpdate()
    {
        if (_effObj == null)
            return ArtActionState.ASS_FAILURE;

        _effObj.transform.position = _playerChildObj.position;
        _effObj.transform.rotation = _playerChildObj.rotation;

        if (_effObj.isPlaying)
            return ArtActionState.AAS_RUNNING;
        else if (_effObj.isStopped)
            return ArtActionState.AAS_END;
        else
            return ArtActionState.AAS_SUCCEED;
    }

    public string GetEffectName()
    {
        if (_args.Count == 0) return "";
        return Convert.ToString(_args[0]);
    }

}
