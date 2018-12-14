using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Grenade : Weapon {
    private GameObject _grenadePrefabs = null;
    public Weapon_Grenade(int weaponId, Avatar obj = null) : base(weaponId, obj)
    {

    }

    public override void ShootKey(Transform shootPointObj, GameObject weaponParent)
    {
        if (!_grenadePrefabs)
        {
            string path = GameManager.Instance.GetBulletProPerty(WeaponProperty.BulletId).PrefabsPath;
            _grenadePrefabs = Resources.Load(path) as GameObject;
        }

        if (_grenadePrefabs)
        {
            //1)加载产生子弹
            GameObject grenadePrefabObj = GameObject.Instantiate(_grenadePrefabs, shootPointObj.position, shootPointObj.rotation) as GameObject; 

            Bullet buttleObj = grenadePrefabObj.GetComponent<Bullet>();
            buttleObj.Init(WeaponProperty.BulletId, weaponParent);
            buttleObj.SetSplatData(SplatId++, WeaponProperty.ShootRange, ColorMask);
            if (SplatId == SplatManagerSystem.instance.SplatSample_X.Count)
            {
                SplatId = 0;
            }

            Rigidbody r = buttleObj.GetComponent<Rigidbody>();
            r.velocity = shootPointObj.forward * WeaponProperty.ShootSpeed;
            //  Debug.Log("shootPointObj.parent::" + data + ",shootPointObj.parent.name:::" + shootPointObj.parent.name);
            //   Debug.Log("Rigidbody.forward:::" + shootPointObj.forward);

            //2)相对应能量的消耗
            ConsumeEnergy(WeaponProperty.MinConsumeEnergy);


        }
    }

    public override void ShootKeyUp()
    {
  
    }


    public override void Release()
    {
        _grenadePrefabs = null;
    }

}
