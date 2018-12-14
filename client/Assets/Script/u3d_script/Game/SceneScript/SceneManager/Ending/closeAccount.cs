using GameLogic.Configs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class closeAccount : MonoBehaviour {

    // Use this for initialization
    void Start ()
    {
        StartCoroutine(EnterGame());
    }
	
	// Update is called once per frame
	void Update ()
    {

	}

    //统计结果
    public void statiscalResult()
    {
        RenderTexturePool.GetInstance().StatisticalResult();
        Vector2 scoresTotal = RenderTexturePool.GetInstance().scoresTotal;
        Debug.Log("scoresTotal:::"+ scoresTotal.x + scoresTotal.y);
        if (scoresTotal == Vector2.zero)
        {
            KBEngine.Event.fireIn("statisticalResult", GameManager.Instance.TeamId, 50f);
        }
        else
        {
            float redData  = (scoresTotal.x / (scoresTotal.x + scoresTotal.y)) * 100;
            float blueData = (scoresTotal.y / (scoresTotal.x + scoresTotal.y)) * 100;
            if (redData == 0) { redData = 100 - blueData; }
            else if (blueData == 0) { blueData = 100 - redData; }
            if (GameManager.Instance.TeamId  == CommonConfigs.RED_TEAM_ID)
                KBEngine.Event.fireIn("statisticalResult", CommonConfigs.RED_TEAM_ID, redData);
            else
                KBEngine.Event.fireIn("statisticalResult", CommonConfigs.BLUE_TEAM_ID, blueData);
        }
       
      //  KBEngine.Event.fireIn("statisticalResult", (byte)1, 65.4f);
    }

    IEnumerator EnterGame()
    {
        yield return new WaitForSeconds(2f);
        statiscalResult();
    }

}
