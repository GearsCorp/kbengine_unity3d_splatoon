using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCreation : MonoBehaviour
{
    //右遥感参数
    private bool _dragging = false;   //标记是否鼠标在滑动 
    bool TouchMouse = false;
    private bool _directorToLeft = false;
    private bool _directorToRight = false;
    private Vector2 _firstPoint = Vector2.zero;    //记录鼠标点击的初始位置  
    private Vector2 _secondPoint = Vector2.zero;    //记录鼠标移动时的位置  

    public  float SelectDur  = 5f;
    private float _dragLenth = 0f;

    public GameObject[] PrefabsObjets;  //供选择的角色集合
    private GameObject[] RoleObjets;  //供选择的角色集合

    // Use this for initialization

    private int _selectedIndex = 0;
    void Awake()
    {
        RoleObjets = new GameObject[PrefabsObjets.Length];
        for (int index = 0; index < PrefabsObjets.Length; index++)
            RoleObjets[index] = GameObject.Instantiate(PrefabsObjets[index], transform.position, transform.rotation) as GameObject;
    }
	 
	// Update is called once per frame
	void Update ()
    {
        if (Mathf.Abs(_dragLenth) >= SelectDur && _dragging) 
        {
         //   Debug.Log("_dragLenth::"+ _dragLenth);
            if ((_dragLenth / SelectDur) > 0)
            {
                checkSelectRoleIndex(true);
            }
            else {
                checkSelectRoleIndex(false);
            }
            _dragLenth = 0f;
        }

        UpdateCharacterRoleShow();
    }

    void UpdateCharacterRoleShow()
    {
        if (RoleObjets.Length <= 0) return;
       
        RoleObjets[_selectedIndex].SetActive(true);
        for (int index = 0; index < RoleObjets.Length; index++) {
            if (index != _selectedIndex) {
                RoleObjets[index].SetActive(false);
            }
        }
        GameManager.Instance.SelectRoteId = (UInt64)_selectedIndex;
    }

    void checkSelectRoleIndex(bool subflag)
    {
        if (RoleObjets.Length <= 1 ) return;
        if (subflag && 0 == _selectedIndex)
        {
            _selectedIndex = RoleObjets.Length-1;
        }
        else if (!subflag && (RoleObjets.Length-1) == _selectedIndex)
        {
            _selectedIndex = 0;
        }
        else {
            if (subflag) _selectedIndex--;
            else _selectedIndex++;
        }
    }

    void OnGUI()
    {
        if (TouchMouse) return;

        if (Event.current.type == EventType.MouseDown)
        {   //记录鼠标按下的位置    
            _firstPoint = Event.current.mousePosition;
            _dragLenth = 0f;
        }
        if (Event.current.type == EventType.MouseDrag)
        {   //记录鼠标拖动的位置    

            _dragging = true;
            _secondPoint = Event.current.mousePosition;
            Vector2 slideDirection = _secondPoint - _firstPoint;
            float x = slideDirection.x, y = slideDirection.y;

            if (y < x && y > -x) // right         
            {
                _directorToLeft = false;
                _directorToRight = true;
            }
            else if (y > x && y < -x)// left  
            {
                _directorToLeft = true;
                _directorToRight = false;
            }
            else
            {
                _directorToLeft = false;
                _directorToRight = false;
            }

            if (_directorToRight || _directorToLeft) {
                _dragLenth += (_secondPoint.x - _firstPoint.x);
            }
           
            _firstPoint = _secondPoint;
            
        }
        else
        {
            _dragging = false;
        }
    }


}
