using GameLogic.ComProperty;
using KBEngine;
using UnityEngine;
public class Weapon_B : Weapon
{
    private FP _accumulateTime = 0f;   //蓄力时间

    private Vector3 _shootForward = Vector3.zero;
    private Quaternion _shootRotation;
    private Vector3 _shootPosition;
    private GameObject _weaponParent;
    private GameObject _bulletPrefabs = null;


    public Weapon_B(int weaponId, Avatar obj = null) : base(weaponId, obj)
    {
        
    }
    public override void ShootKey(Transform shootPointObj, GameObject weaponParent)
    {
        Debug.Log("vvvvvvvvvvvvvvvvv::");
        _shootForward  = shootPointObj.forward;
        _shootRotation = shootPointObj.rotation;
        _shootPosition = shootPointObj.position;
        _weaponParent  = weaponParent;

        if (_accumulateTime >= WeaponProperty.MaxAccumulateForceTime)
        {
            CreateBullet();
            return;
        }

        _accumulateTime += Time.deltaTime;
    }

    public override void ShootKeyUp()
    {
        if (_shootForward != Vector3.zero && _accumulateTime > 0)
        {
            int bulletCount = (int)WeaponProperty.MinAccumulateForce +
                (int)(_accumulateTime * WeaponProperty.MaxAccumulateForce);

            CreateBullet();
        }
    }

    private void CreateBullet(int bulletCount)
    {
        float initSpeed = WeaponProperty.ShootSpeed;
        if (!_bulletPrefabs)
        {
            string path = GameManager.Instance.GetBulletProPerty(WeaponProperty.BulletId).PrefabsPath;
            _bulletPrefabs = Resources.Load(path) as GameObject;
        }
       
        for (int i = 0; i < bulletCount; i++)
        {
            if (_bulletPrefabs)
            {
                if (i == bulletCount - 1)
                {

                
                //1)加载产生子弹
                GameObject BulletPrefabObj = GameObjPool.GetInstance().InstantiateObject(_bulletPrefabs, _shootPosition, _shootRotation);
                Bullet buttleObj = BulletPrefabObj.GetComponent<Bullet>();
                buttleObj.Init(WeaponProperty.BulletId, _weaponParent);
                buttleObj.SetSplatData(SplatId++, 8, ColorMask);
                if (SplatId == SplatManagerSystem.instance.SplatSample_X.Count)
                {
                    SplatId = 0;
                }
                Rigidbody r = buttleObj.GetComponent<Rigidbody>();
                r.velocity = _shootForward * initSpeed;

                }
                //2)武器相对应的子弹库存数操作
                if (WeaponProperty.HP != 0)
                    WeaponProperty.HP--;
                initSpeed += 1;
            }
        }
       
        ResetData();
    }

    private void CreateBullet()
    {
        if (_accumulateTime > WeaponProperty.MaxAccumulateForceTime)
            _accumulateTime = WeaponProperty.MaxAccumulateForceTime;
        float initSpeed = WeaponProperty.ShootSpeed;
        if (!_bulletPrefabs)
        {
            string path = GameManager.Instance.GetBulletProPerty(WeaponProperty.BulletId).PrefabsPath;
            _bulletPrefabs = Resources.Load(path) as GameObject;
        }

        //获取伤害值
        int Damage = GetDamage(WeaponProperty.BulletId, WeaponProperty.MaxAccumulateForceTime);
        int ShootRange  = GetShootRange();  //获取射程
        int EnergyValue = GetConsumeEnergy(); //获取消耗的能量值
        Debug.Log("Damage::"+ Damage + ",ShootRange::"+ ShootRange + ",EnergyValue::"+ EnergyValue);
        //for (int i = 0; i < 10; i++)
        //{
        //    if (_bulletPrefabs)
        //    {
        //        if (i == 10 - 1)
        //        {


        //1)加载产生子弹
        GameObject BulletPrefabObj = GameObjPool.GetInstance().InstantiateObject(_bulletPrefabs, _shootPosition, _shootRotation);
                    Bullet buttleObj = BulletPrefabObj.GetComponent<Bullet>();
                    buttleObj.Init(WeaponProperty.BulletId, _weaponParent,Damage);
                    buttleObj.SetSplatData(SplatId++, ShootRange/3, ColorMask);
                    if (SplatId == SplatManagerSystem.instance.SplatSample_X.Count)
                    {
                        SplatId = 0;
                    }
                    Rigidbody r = buttleObj.GetComponent<Rigidbody>();
                    r.velocity = _shootForward * ShootRange;

            //2)相对应能量的消耗
            ConsumeEnergy(EnergyValue);

        //}
        ////2)武器相对应的子弹库存数操作
        //if (WeaponProperty.HP != 0)
        //    WeaponProperty.HP--;
        //initSpeed += 1;
        //    }
        //}

        ResetData();
    }

    private void ResetData()
    {
        _shootForward = Vector3.zero;
        _shootPosition = Vector3.zero;
        _accumulateTime = 0;
        _weaponParent = null;
    }

    public override void Release()
    {
        _weaponParent = null;
        _bulletPrefabs = null;
    }

    /// <summary>
    /// 获取伤害值
    /// </summary>
    /// <param name="id"></param>
    /// <param name="BulletData"></param>
    /// <param name="maxAccumulateTime"></param>
    /// <returns></returns>
    public int GetDamage(int id, FP maxAccumulateTime)
    {
        BulletProperty BulletData = GameManager.Instance.GetBulletProPerty(id);
        FP damageTime =  maxAccumulateTime/(BulletData.MaxDamage - BulletData.Damage);
        int damageNum = (int)(_accumulateTime/ damageTime);
        return (BulletData.Damage + damageNum);
    }

    /// <summary>
    /// 获取射程
    /// </summary>
    /// <returns></returns>
    public int GetShootRange()
    {
        FP rangeTime = WeaponProperty.MaxAccumulateForceTime / (WeaponProperty.MaxShootRange - WeaponProperty.ShootRange);
        int rangeNum = (int)(_accumulateTime / rangeTime);
        return (rangeNum + WeaponProperty.ShootRange);
    }

    public int GetConsumeEnergy()
    {
        FP  enerayTime = WeaponProperty.MaxAccumulateForceTime / (WeaponProperty.MaxConsumeEnergy - WeaponProperty.MinConsumeEnergy);
        int enerayNum = (int)(_accumulateTime / enerayTime);
        return (enerayNum + WeaponProperty.MinConsumeEnergy);
    }
}
