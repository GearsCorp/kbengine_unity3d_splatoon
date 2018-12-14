using GameLogic.ComProperty;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponOption : MonoBehaviour {

    // Use this for initialization
    private int _curWeapon;
    private GameObject _curWeaponPrefabs;
    private GameObject _curWeaponObj;
    public Transform CurWeaponPosition;
    public Transform WeaponPosition_0;
    public Transform WeaponPosition_1;
    public Transform WeaponPosition_2;
    public Transform WeaponPosition_3;

    public int CurPageSelect = 0;
    private void Awake()
    {
        _curWeapon = GameManager.Instance.CurWeapon;
        Debug.Log("_curWeapon_curWeapon_curWeapon_curWeapon::" + _curWeapon);
        InitAllWeapons();
        CurWeaponShow();
        Debug.Log(" GameManager.Instance.weapon.count::" + GameManager.Instance.WeaponProPertyDic.Count);
    }
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {

        if (_curWeapon != GameManager.Instance.CurWeapon)
        {
            if(_curWeaponObj)
               DestroyImmediate(_curWeaponObj);
            _curWeapon = GameManager.Instance.CurWeapon;
            CurWeaponShow();
        }
        
    }

    void OnDestroy()
    {
        Destroy(_curWeaponObj);
        Destroy(_curWeaponObj);
        Destroy(CurWeaponPosition);
        Destroy(WeaponPosition_0);
        Destroy(WeaponPosition_1);
        Destroy(WeaponPosition_2);
        Destroy(WeaponPosition_3);
        Resources.UnloadUnusedAssets();
    }

    void InitAllWeapons()
    {
        Debug.Log("GameManager.Instance.WeaponQueue.Count::"+ GameManager.Instance.WeaponQueue.Count);
        //武器个数
        for (int count = 0; count < GameManager.Instance.WeaponQueue.Count; count++)
        {
            int id = GameManager.Instance.WeaponQueue[count];
            GameObject curObj = Resources.Load(GameManager.Instance.GetWeaponProPerty(id).PrefabsPath) as GameObject;
            if (curObj)
            {
                Transform positionObj = WeaponPosition_0;
                if (count == 1)
                    positionObj = WeaponPosition_1;
                if (count == 2)
                    positionObj = WeaponPosition_2;
                if (count == 3)
                    positionObj = WeaponPosition_3;
                GameObject.Instantiate(curObj, positionObj.position,positionObj.rotation);
            }
            else
            {
                Debug.Log("CurWeapon path is error!");
            }
        }
    }

    void CurWeaponShow()
    {
        //当前武器属性
        WeaponProperty PropertyData = GameManager.Instance.GetWeaponProPerty(_curWeapon);
        if (!PropertyData.IsNull()) {
            string path = PropertyData.PrefabsPath;
            _curWeaponPrefabs = Resources.Load(path) as GameObject;
         
            if (_curWeaponPrefabs)
            {
                _curWeaponObj = Instantiate(_curWeaponPrefabs, CurWeaponPosition.position, CurWeaponPosition.rotation);
            }
            else {
                Debug.Log("CurWeapon path is error!");
            }  
        }
    }


}
