using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testAnimator : MonoBehaviour {

    public Animator animator;
    // Update is called once per frame 
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            animator.speed = 0;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            animator.speed = 1;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Time.timeScale = 0;
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            Time.timeScale = 1;
        }

    } 

}
