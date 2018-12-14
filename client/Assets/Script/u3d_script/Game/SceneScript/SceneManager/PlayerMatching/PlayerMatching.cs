using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PlayerMatching : MonoBehaviour
{
    public Image ProgressImage;  //转动的进度条
    public float RotateSpeed = 50f;
    private void Awake()
    {
 
    }

    void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.ifStartBattle())//转动进度条e
        {
            ProgressImage.transform.Rotate(new Vector3(0, 0, RotateSpeed * Time.deltaTime));
        }
    }

}
