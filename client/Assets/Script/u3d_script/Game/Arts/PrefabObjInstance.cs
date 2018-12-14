using CBFrame.Sys;
using GameLogic.Configs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabObjInstance : CBArtAction
{
    private Transform _playerChildObj;
    private GameObject _effObject;
    private float _destroyTime;
    public PrefabObjInstance(List<object> args, GameObject obj) : base(args, obj)
    {
        _playerChildObj = Go.transform.Find(PlayerCommonName.ChildName);
    }

    protected override void OnBegin()
    {
        string name = GetPrefabName();
        Debug.Log("PrefabObjInstance::OnBegin::" + name);
        if (name == "") return;
        _effObject = (GameObject)Resources.Load(name);
        if (_effObject == null) return;
        _effObject = MonoBehaviour.Instantiate(_effObject, _playerChildObj.position, _playerChildObj.rotation);

        _destroyTime = GetDestoryTime();
    }

    protected override void OnEnd()
    {
        if (_effObject)
        {
            MonoBehaviour.Destroy(_effObject);
            _effObject = null;
            _destroyTime = GetDestoryTime();
            Debug.Log("PrefabObjInstance:::_OnEnd!!!!");
        }
    }

    protected override void OnReset()
    {
    }

    protected override ArtActionState OnUpdate()
    {
        if (_effObject == null)
            return ArtActionState.ASS_FAILURE;

        _effObject.transform.position = _playerChildObj.position;
        _effObject.transform.rotation = _playerChildObj.rotation;
        if (GetDestoryTime() == 1000)
            return ArtActionState.AAS_RUNNING;
        else if(_destroyTime <= 0.000001f)
            return ArtActionState.AAS_END;
        else
        {
            _destroyTime -= Time.deltaTime;
            return ArtActionState.AAS_RUNNING;
        }

    }

    IEnumerator DestoryInstance()
    {
        yield return new WaitForSeconds(GetDestoryTime());
    }
    public string GetPrefabName()
    {
        if (_args.Count == 0) return "";
        return Convert.ToString(_args[0]);
    }

    //获取毁灭时间
    public float GetDestoryTime()
    {
        if (_args.Count > 2 || Convert.ToInt32(_args[1]) < 0) return 1000;
        return Convert.ToInt32(_args[1]);
    }

    //设置
    public virtual void SetGameObject(Transform obj)
    {
        _playerChildObj = obj;
    }
}
