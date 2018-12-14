using GameLogic.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameLogic.Configs;

public class resultShow : MonoBehaviour {

    // Use this for initialization
    public Text RedText;
    public Text BlueText;

    public Image RedImage;
    public Image BlueImage;

    bool _imageBattle = true;
    float _step1 = 0.0f;
    float _step2 = 0.0f;
    float _step3 = 0.0f;
    float _step4 = 0.0f;


    private void Awake()
    {
        KBEngine.Event.registerOut(AvatarEvent_Out.EventName.onStatisticalResult, this, "onStatisticalResult");
    }
    void Start () {
       
	}

    private void OnDestroy()
    {
        KBEngine.Event.deregisterOut(this);
    }
    // Update is called once per frame
    void Update ()
    {

        if (_imageBattle)
        {
            //这里要实现进度条对冲的UI实现
            PreSimulate();
        }
    }

    void PreSimulate()
    {
        if (!_imageBattle) return;

        if (_step1 <= 1.0f)
        {
            _step1 = Mathf.Clamp01(_step1 += Time.deltaTime);
            RedImage.fillAmount = Mathf.Lerp(0.0f, 0.5f, _step1);
            BlueImage.fillAmount = Mathf.Lerp(0.0f, 0.5f, _step1);
          //  Debug.Log("ssssss:::;"+ _step1);
        }

        if (_step2 <= 1.0f && _step1 >= 1.0f)
        {
            _step2 = Mathf.Clamp01(_step2 += Time.deltaTime);
            RedImage.fillAmount = Mathf.Lerp(0.5f, 0.4f, _step2);
            BlueImage.fillAmount = Mathf.Lerp(0.5f, 0.6f, _step2);
        }

        if (_step3 <= 1.0f && _step2 >= 1.0f)
        {
            _step3 = Mathf.Clamp01(_step3 += Time.deltaTime);
            RedImage.fillAmount  = Mathf.Lerp(0.4f, 0.6f, _step3);
            BlueImage.fillAmount = Mathf.Lerp(0.6f, 0.4f, _step3);
        }

        if (_step4 <= 1.0f && _step3 >= 1.0f)
        {
            _step4 = Mathf.Clamp01(_step4 += Time.deltaTime );
            RedImage.fillAmount = Mathf.Lerp(0.6f, 0.5f, _step4);
            BlueImage.fillAmount = Mathf.Lerp(0.4f, 0.5f, _step4);
        }

        if (_step4 >= 1.0f && _imageBattle)
        {
            _step1 = 2f;
            _step2 = 0.0f;
            _step3 = 0.0f;
            _step4 = 0.0f;
        }
        UpdateResultShowText();
    }

    void UpdateResultShowText()
    {
     //   Debug.Log("RedImage::"  + RedImage.fillAmount);
     //   Debug.Log("BlueImage::" + BlueImage.fillAmount);
        double data = Math.Round(RedImage.fillAmount * 100, 1);
        RedText.text  = (data).ToString() + "%";
        BlueText.text = (100 - data).ToString() + "%";
   
    }

    //public void SetValue(float _value)
    //{
    //    string value = string.Format("{0:F3}", ((int)(_value * 1000) / 1000.0f));

    //    for (int i = 0; i < Count; i++)
    //    {
    //        string number = value.Substring(value.Length - (i + 1), 1);
    //        mNum[Count - 1 - i].sprite = Resources.Load(GameDefine.FightClockNumPrefix + number, typeof(Sprite)) as Sprite;
    //    }
    //}


    public void onStatisticalResult(Byte teamId, float value)
    {
        Debug.Log("resultShow_resultShow_resultShow_onStatisticalResult");
        if (teamId == CommonConfigs.RED_TEAM_ID)
        {
            RedText.text  = value.ToString() + "%";
            BlueText.text = (100f - value).ToString() + "%";
            RedImage.fillAmount = value/100;
            BlueImage.fillAmount = (1 - value / 100);
        }
        else
        {
            RedText.text = (100f - value).ToString() + "%";
            BlueText.text = value.ToString() + "%";
            BlueImage.fillAmount = value / 100;
            RedImage.fillAmount = (1 - value / 100);
        }

        //这里要处理最终值的Image
        _imageBattle = false;
     
        //Invoke("endOfStatistics",2f);
        StartCoroutine(EndOfStatistics());
    }


    public void endOfStatistics()
    {
        //向服务器说明统计结束
        KBEngine.Event.fireIn(AvatarEvent_In.EventName.endOfStatistics);
        Debug.Log("EndScene_fireIn::AvatarEvent_In.EventName.endOfStatistics");
    }

    IEnumerator EndOfStatistics()
    {
        yield return new WaitForSeconds(2f);
        //向服务器说明统计结束
        KBEngine.Event.fireIn(AvatarEvent_In.EventName.endOfStatistics);
        Debug.Log("EndScene_fireIn::AvatarEvent_In.EventName.endOfStatistics");
    }
}
