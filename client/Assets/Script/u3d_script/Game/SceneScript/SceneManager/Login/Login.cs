using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameLogic.Events;
using System;
using KBEngine;
using UnityEngine.SceneManagement;
using GameLogic.Configs;

public class Login : MonoBehaviour
{

    public Text NameTextObj;
    public Text PwdTextObj;
	// Use this for initialization
	void Start ()
    {
        GetComponent<Button>().onClick.AddListener(OnLoginClick);
        installEvents();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if ((NameTextObj && NameTextObj.text == "") || 
            (PwdTextObj && PwdTextObj.text == ""))
        {
            this.enabled = false;
        }
        else {
            this.enabled = true;
        }
    }

    public void OnLoginClick()
    {
        //向服务器发送登录数据
        Debug.Log("connect to server...(连接到服务端...)"+"NameTextObj.text::"+ NameTextObj.text + ",PwdTextObj.text::"+ PwdTextObj.text);
        KBEngine.Event.fireIn(AccountEvent_In.EventName.login, NameTextObj.text, PwdTextObj.text, System.Text.Encoding.UTF8.GetBytes("inkfish_demo"));
    }

    public void installEvents()
    {
        KBEngine.Event.registerOut(AccountEvent_Out.EventName.onLoginFailed, this, "onLoginFailed");
        KBEngine.Event.registerOut(AccountEvent_Out.EventName.onLoginBaseappFailed, this, "onLoginBaseappFailed");
        KBEngine.Event.registerOut(AccountEvent_Out.EventName.onLoginSuccessfully, this, "onLoginSuccessfully");
    }

    private void OnDestroy()
    {
        KBEngine.Event.deregisterOut(this);
        NameTextObj = null;
        PwdTextObj = null;
    }
    public void onLoginFailed(UInt16 failedcode)
    { 
        if (failedcode == 20)  
        {
           Debug.Log("login is failed(登陆失败), err=" + KBEngineApp.app.serverErr(failedcode) + ", " + System.Text.Encoding.ASCII.GetString(KBEngineApp.app.serverdatas()));
        }
        else
        {
            Debug.Log("login is failed(登陆失败), err=" + KBEngineApp.app.serverErr(failedcode));
        }
    }

    public void onLoginBaseappFailed(UInt16 failedcode)
    {
        Debug.Log("failedcode:"+ KBEngineApp.app.serverErr(failedcode));
    }

    public void onLoginSuccessfully() 
    {
        SceneManager.LoadScene(SceneComNames.StartScene, LoadSceneMode.Single);
        Debug.Log("Login_Scene Clicked. ClickHandler.");
    }
}
 