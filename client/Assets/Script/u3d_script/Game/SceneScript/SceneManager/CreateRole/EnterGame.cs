using GameLogic.Configs;
using GameLogic.Events;
using KBEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnterGame : MonoBehaviour {

    public InputField InputObj;
    // Use this for initialization
    public bool bExitRole = false;
    void Start()
    {
       GetComponent<Button>().onClick.AddListener(OnClick);

        // 触发登陆成功事件
       // KBEngine.Event.fireOut(AccountEvent_In.EventName.reqAvatarList);
        KBEngine.Event.registerOut(AccountEvent_Out.EventName.onReqCreateAvatar, this, "onReqCreateAvatar");
        bExitRole = (GameManager.Instance.GetAvatarBaseInfos().Count > 0) ? true : false;
    }

    private void OnDestroy()
    {
        KBEngine.Event.deregisterOut(this);
    }

    private void Update()
    {
        ControllUI();
    }
 
    // Update is called once per frame
    void OnClick()
    {
        if (InputObj)//此时的角色类型暂时默认为1
        {
            if (bExitRole)
            {
                changeScene();
            }
            else
            {
                Byte roleType = 1;
                KBEngine.Event.fireIn("reqCreateAvatar", roleType, InputObj.text);
            }
          
        }
    }

    /// <summary>
    /// 根据不同的情况控制UI的显示
    /// </summary>
    void ControllUI()
    {
        if (InputObj == null) return;
        if (GameManager.Instance.GetAvatarBaseInfos() == null) Debug.Log("GameManager.Instance.GetAvatarBaseInfos is null");
        if (GameManager.Instance.GetAvatarBaseInfos().Count > 0)
        {
            UInt64 keyId = 0;
            foreach (UInt64 key in GameManager.Instance.GetAvatarBaseInfos().Keys)
            {
                keyId = key;
                break;
            }
            InputObj.text = GameManager.Instance.GetAvatarBaseInfo(keyId).name;
            GetComponent<Button>().interactable = true;
            InputObj.readOnly = true;
        }
        else {
            InputObj.readOnly = false ;
            if (InputObj.text != "")
            {
                GetComponent<Button>().interactable = true;
            }
            else if (InputObj.text == "")
            {
                GetComponent<Button>().interactable = false;
            }
        }
    }

    public void onReqCreateAvatar(AVATAR_INFOS arg2)
    {
        changeScene();
    }
    public void changeScene()
    {
        SceneManager.LoadScene(SceneComNames.GameRoom, LoadSceneMode.Single);
        Debug.Log("GameRoom_Scene Clicked. ClickHandler.");
        //向服务器发送进入游戏大厅的消息
        KBEngine.Event.fireIn(AccountEvent_In.EventName.enterGameRoom);
    }
}
