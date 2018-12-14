using GameLogic.Configs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInstance : PrefabObjInstance
{
    public WeaponInstance(List<object> args, GameObject obj) : base(args, obj)
    {
        // string objName = PlayerCommonName.QianKouName + GameManager.Instance.GetWeaponProPerty(GameManager.Instance.CurWeapon).WeaponName;
        string objName = PlayerCommonName.QiangKouName + "武器A";

        SetGameObject(GameObject.Find(objName).transform);
    }
}
