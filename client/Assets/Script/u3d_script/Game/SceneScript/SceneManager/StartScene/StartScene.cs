using GameLogic.Configs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0))
        {
            SceneManager.LoadScene(SceneComNames.CreateRole, LoadSceneMode.Single);
            Debug.Log("StartScene_Scene Clicked. ClickHandler.");
        }
	}
}
