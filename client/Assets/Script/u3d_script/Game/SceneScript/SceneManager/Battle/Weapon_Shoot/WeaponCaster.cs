using CBFrame.Sys;
using GameLogic.ComProperty;
using GameLogic.Configs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCaster : MonoBehaviour
{
    public GameObject BackPackObj;
    private Weapon _weaponObj;       //当前选择的武器对象
    private Dictionary<int, Weapon> WeaponDic = new Dictionary<int, Weapon>(); //保存武器数组

    private Transform _shootPoint;     //开枪点
 
    public GameObject _weaponParent;   //武器所属的玩家对象 
    private Avatar _avatarObj = null;  //玩家身上的avatar属性

    private int _restoreBulletSpeed;   //恢复子弹的速度

    private Coroutine _shootInvokeFun = null;   //配合开枪的动作延时的携程

    private Coroutine _restoreBulletsFun = null;


    public  Transform ShootPoint_point; //开枪点

    public GameObject childParent; //开枪点

    public Weapon CurrentWeapon
    {
        get
        {
            return _weaponObj;
        }

        set
        {
            _weaponObj = value;
        }
    }

    private Weapon_Grenade _grenadeWeapon = null;
    public int RestoreBulletSpeed
    {
        get
        {
            return _restoreBulletSpeed;
        }

        set
        {
            _restoreBulletSpeed = value;
        }
    }

    // private GameObject _canvasGameObj;

    // Use this for initialization
    public Color WeaponColor = Color.white;
    void Start ()
    {
        if (_weaponParent)
        {
            _avatarObj = _weaponParent.transform.GetComponent<Avatar>();
        }
        //test.....test//
       // GameManager.Instance.CurWeapon = 1000;
       // ConfiguringCurrentProPerities(1000);
        //string path = CurrentWeapon.WeaponProperty.WeaponName + "/" + PlayerCommonName.QiangKouName + CurrentWeapon.WeaponProperty.WeaponName;
        //_shootPoint = gameObject.transform.Find(path);
       // ConfiguringCurrentProPerities(GameManager.Instance.CurWeapon);
        _shootPoint = ShootPoint_point;
        RestoreBulletSpeed = WeaponComConfig.NOR_RESTORE_SPEED;
    }
    private void OnDestroy()
    {
        Destroy(_shootPoint);
        Destroy(_weaponParent);
        if (_weaponParent && _shootInvokeFun != null)
            StopCoroutine(_shootInvokeFun);

        WeaponDic.Clear();
    }
    private void Update()
    {
        ////test.....test//
        //if (TestBegin)
        //    ChangeData();
    }


    public void Shoot(bool flag)
    {
        if (flag && CurrentWeapon != null)
        {
            CurrentWeapon.ShootKey(_shootPoint, _weaponParent);
        }
        else if (CurrentWeapon != null)
        {
           CurrentWeapon.ShootKeyUp();
        }
    }

    public void ThrowGrenade(bool flag)
    {
        if (_grenadeWeapon == null)
        {
            _grenadeWeapon = Activator.CreateInstance(Type.GetType("Weapon_Grenade"), GameManager.Instance.GrenadesId, _avatarObj) as Weapon_Grenade;
        }

        if(flag && _grenadeWeapon != null)
             _grenadeWeapon.ShootKey(_shootPoint, _weaponParent);
        else if(_grenadeWeapon != null)
            _grenadeWeapon.ShootKeyUp();
    }

    /// <summary>
    /// 配置当前属性
    /// </summary>
    /// <param name="id"></param>
    public void ConfiguringCurrentProPerities(int id)
    {
        if (_weaponParent && !_avatarObj)
        {
            _avatarObj = _weaponParent.transform.GetComponent<Avatar>();
        }
        if (!WeaponDic.ContainsKey(id))
        {
            string weaponName = GameManager.Instance.GetWeaponProPerty(id).WeaponName;
            Debug.Log("weaponName::" + weaponName + ",id::" + id);
            string lastName   = weaponName.Substring(weaponName.Length - 1);
            weaponName = "Weapon_" + lastName;
            CurrentWeapon = Activator.CreateInstance(Type.GetType(weaponName), id, _avatarObj) as Weapon;
            WeaponDic.Add(id, CurrentWeapon);
            // Debug.Log("Type.GetType(weaponName):::" + weaponName + ",isNUll::" + (_weaponObj == null));
        }
        else
        {
            CurrentWeapon = WeaponDic[id];
        }
        if (WeaponColor != Color.white && CurrentWeapon != null)  //更换武器的时候，将武器喷漆颜色的设置上
        {
            SetWeaponInkColor(WeaponColor);
        }
        Debug.Log("CurrentWeapon:::"+ CurrentWeapon.WeaponProperty.WeaponName + ",, id::"+CurrentWeapon.WeaponProperty.BulletId);
    }

    /// <summary>
    /// 返回指定武器最低的消耗能量数值
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public int GetMinEnergofWeapon(int id = -1)
    {
        if (id == -1)
        {
            return CurrentWeapon.WeaponProperty.MinConsumeEnergy;
        }
        else
        {
            if (!WeaponDic.ContainsKey(id))
            {
                return -1;
            }
            else
            {
                return GameManager.Instance.GetWeaponProPerty(id).MinConsumeEnergy;
            }
        }
    }


    /// <summary>
    /// 设置武器喷漆颜色
    /// </summary>
    /// <param name="color"></param>
    public void SetWeaponInkColor(Color color, int id = 0)
    {
        WeaponColor = color;
        if (CurrentWeapon == null)
        {
            ////test.....test//
            //GameManager.Instance.CurWeapon = 1000;
            //ConfiguringCurrentProPerities(1000);
            ////else....
            int WeaponId = id;
            if (0 == id) WeaponId = GameManager.Instance.CurWeapon;
            ConfiguringCurrentProPerities(WeaponId);
            ChangeWeaponMeshRender(CurrentWeapon.WeaponProperty.WeaponName);

            //Debug.Log("GameManager.Instance.CurWeapon:::"+ GameManager.Instance.CurWeapon +
            //       ",,urrentWeapon.WeaponProperty.WeaponName::" + CurrentWeapon.WeaponProperty.WeaponName +
            //       ",,CurrentWeapon.WeaponProperty.BulletId::" + CurrentWeapon.WeaponProperty.BulletId);
        }
        else
        {
            CurrentWeapon.SetWeaponColor(color);
        }
       
    }

    
    /// <summary>
    /// 更换武器
    /// </summary>
    public void ChangeWeaponEvent(int WeaponId)
    {
        if (CurrentWeapon != null && CurrentWeapon.WeaponProperty.Id == WeaponId) return;
        string beforeWeaponName = CurrentWeapon.WeaponProperty.WeaponName;
        ConfiguringCurrentProPerities(WeaponId);
        string weaponName = CurrentWeapon.WeaponProperty.WeaponName;
        ChangeWeaponMeshRender(weaponName, beforeWeaponName);
    }

    /// <summary>
    /// 更换武器后，改变武器的meshRender
    /// </summary>
    public void ChangeWeaponMeshRender(string lastName, string beforeName = "")
    {
        if (beforeName == "")
        {
            int i = 0;
            Debug.Log("gameObject.transform.childCountgameObject.transform.childCount::" + gameObject.transform.childCount + ", gameObject.transform.childCount::"+ BackPackObj.transform.childCount);
            while (i < gameObject.transform.childCount)
            {
                gameObject.transform.GetChild(i).gameObject.SetActive(false);
                i++;
            }
            i = 0;

            int j = 0;
            bool initFlag = false;
            while (i < BackPackObj.transform.childCount)
            {
                BackPackObj.transform.GetChild(i).gameObject.SetActive(false);
               if (lastName != BackPackObj.transform.GetChild(i).gameObject.name && !initFlag)
               {
                    j = i;
                    initFlag = true;
               }
                i++;
            }
            gameObject.transform.Find(lastName).gameObject.SetActive(true);
            BackPackObj.transform.GetChild(j).gameObject.SetActive(true);
        }
        else
        {
            //更换手持武器样式
            gameObject.transform.Find(beforeName).gameObject.SetActive(false);
            gameObject.transform.Find(lastName).gameObject.SetActive(true);

            //更换背包武器样式
            if (BackPackObj == null) return;
            BackPackObj.transform.Find(lastName).gameObject.SetActive(false);
            BackPackObj.transform.Find(beforeName).gameObject.SetActive(true);
        }
     
    }

    /// <summary>
    /// 初始化武器数据
    /// </summary>
    /// <param name="WeaponId"></param>
    /// <param name="WeaponColor"></param>
    public void InitWeaponData(int WeaponId, Color WeaponColor)
    {
        ConfiguringCurrentProPerities(WeaponId);
        ChangeWeaponMeshRender(CurrentWeapon.WeaponProperty.WeaponName);
        SetWeaponInkColor(WeaponColor);
    }


    #region test测试


    public bool TestColor = true;

    public bool TestBegin = false;
    public void ChangeData()
    {
        if (TestColor)
        {
            SetWeaponInkColor(Color.red);
        }
        else
        {
            SetWeaponInkColor(Color.blue);
        }
    }
    #endregion

}
