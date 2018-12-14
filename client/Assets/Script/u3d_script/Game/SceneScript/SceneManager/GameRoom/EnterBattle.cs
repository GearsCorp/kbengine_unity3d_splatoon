using GameLogic.Configs;
using GameLogic.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnterBattle : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Button>().onClick.AddListener(OnClick);
     //   Invoke("test", 3f);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnClick() 
    {
        SceneManager.LoadScene(SceneComNames.StartGame, LoadSceneMode.Single);
        Debug.Log("StartGame_Scene Clicked. ClickHandler.");
        //向服务器发送开始游戏的消息
        KBEngine.Event.fireIn(AvatarEvent_In.EventName.enterStartGame);
    }

    //void test()
    //{
    //    KBEngine.KBEngineApp.app._args.serverHeartbeatTick = 15;
    //}
}
