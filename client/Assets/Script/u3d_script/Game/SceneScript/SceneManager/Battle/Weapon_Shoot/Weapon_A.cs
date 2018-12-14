using GameLogic.Configs;
using KBEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_A : Weapon
{
    private FP _fireRateTime = 0;   //控制射程时间
    private GameObject _bulletPrefabs = null;



    public Weapon_A(int weaponId, Avatar obj = null) : base(weaponId, obj)
    {

    }

    public override void ShootKey(Transform shootPointObj, GameObject weaponParent)
    {
        //到达设置的规定的射速时间
        if (_fireRateTime >= (1.0f / WeaponProperty.FireRate) || (0 == _fireRateTime))
        {
            Debug.Log("ddddddddddddddddddddddddddddd::+" + WeaponProperty.FireRate);
            if (!_bulletPrefabs)
            {
                string path = GameManager.Instance.GetBulletProPerty(WeaponProperty.BulletId).PrefabsPath;
                _bulletPrefabs = Resources.Load(path) as GameObject;
                _bulletPrefabs.name = PrefabsName.BulletPrefabName;
            }

            if (_bulletPrefabs)
            {
                //1)加载产生子弹
                GameObject BulletPrefabObj =  GameObjPool.GetInstance().InstantiateObject(_bulletPrefabs, shootPointObj.position, shootPointObj.rotation);

                Bullet buttleObj = BulletPrefabObj.GetComponent<Bullet>();
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
            _fireRateTime = 0;
        }
        _fireRateTime += FrameSyncManager.DeltaTime;
    }

    public override void ShootKeyUp()
    {
        _fireRateTime = 0;
    }

    public override void Release()
    {
        _bulletPrefabs = null;
    }

}
