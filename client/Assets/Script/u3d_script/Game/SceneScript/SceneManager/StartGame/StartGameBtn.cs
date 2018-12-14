using GameLogic.Configs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGameBtn : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnClick()
    {
        if (!GameManager.Instance.ifStartBattle())
        {
            SceneManager.LoadScene(SceneComNames.PlayerMatching, LoadSceneMode.Single);
            Debug.Log("PlayerMatching_Scene Clicked. ClickHandler.");
        }
        else
        {
            SceneManager.LoadScene(SceneComNames.Battle, LoadSceneMode.Single);
            Debug.Log("Battle state::" + GameManager.Instance.ifStartBattle()+ 
                ",,Battle_Scene Clicked. ClickHandler.");
        }   
    }


}
