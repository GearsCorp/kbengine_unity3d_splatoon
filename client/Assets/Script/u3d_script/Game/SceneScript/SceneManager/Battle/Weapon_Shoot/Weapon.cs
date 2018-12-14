using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLogic.ComProperty;
public abstract class Weapon 
{
    //武器属性
    private WeaponProperty _weaponProperty;

    //获取贴花的ID
    public int SplatId = 0;

    public Vector4  ColorMask = new Vector4(1, 0, 0, 0);

    private Avatar _avatarObj;
    //设置武器ID
    public Weapon(int weaponId, Avatar obj = null)
    {
        SetWeaponProperty(weaponId);
        _avatarObj = obj;
    }

    ~Weapon()
    {
        Release();
    }

    public WeaponProperty WeaponProperty
    {
        get
        {
            return _weaponProperty;
        }

        set
        {
            _weaponProperty = value;
        }
    }

    /// <summary>
    /// 设置武器颜色
    /// </summary>
    /// <param name="weaponColor"></param>
    public void SetWeaponColor(Color weaponColor)
    {
        ColorMask.x = weaponColor.r;
        ColorMask.y = weaponColor.g;
        ColorMask.z = weaponColor.b;
        ColorMask.w = 0;
        Debug.Log("setWeaponColor:::" + weaponColor);
    }

    public void SetWeaponProperty(int weaponId)
    {
        WeaponProperty = GameManager.Instance.GetWeaponProPerty(weaponId);
    }

    public abstract void ShootKey(Transform shootObj, GameObject weaponParent);

    public abstract void ShootKeyUp();

    public abstract void Release();

    /// <summary>
    /// 消耗能量
    /// </summary>
    /// <param name="energyValue"></param>
    public void ConsumeEnergy(int energyValue)
    {
        if (_avatarObj)
        {
            _avatarObj.EnergyValue -= energyValue;
            //Debug.Log("_avatarObj.EnergyValue:::" + _avatarObj.EnergyValue + " ,energyValue::" + energyValue);
        }
    }

    /// <summary>
    /// 获取玩家身上当前的能量值
    /// </summary>
    /// <returns></returns>
    public int GetCurrentEnergy()
    {
        if (_avatarObj)
        {
           return _avatarObj.EnergyValue;
        }
        return -1;
    }

  
}
