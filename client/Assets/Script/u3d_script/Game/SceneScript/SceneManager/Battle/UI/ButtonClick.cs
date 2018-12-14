using CBFrame.Sys;
using GameLogic.Configs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonClick : MonoBehaviour
{
    public Button ShootBtn;
    public Button JumpBtn;
    public Button GrenadeBtn;

    // Use this for initialization
    void Start()
    {
        EventTriggerListener.Get(ShootBtn.gameObject).onClick   = OnButtonClick;
        EventTriggerListener.Get(JumpBtn.gameObject).onClick    = OnButtonClick;
        EventTriggerListener.Get(GrenadeBtn.gameObject).onClick = OnButtonClick;

        EventTriggerListener.Get(ShootBtn.gameObject).onDown    = OnButtonDown;
        EventTriggerListener.Get(JumpBtn.gameObject).onDown     = OnButtonDown;
        EventTriggerListener.Get(GrenadeBtn.gameObject).onDown  = OnButtonDown;

        EventTriggerListener.Get(ShootBtn.gameObject).onPress   = OnButtonPress;
        EventTriggerListener.Get(JumpBtn.gameObject).onPress    = OnButtonPress;
        EventTriggerListener.Get(GrenadeBtn.gameObject).onPress = OnButtonPress;

        EventTriggerListener.Get(ShootBtn.gameObject).onUp      = OnButtonUp;
        EventTriggerListener.Get(JumpBtn.gameObject).onUp       = OnButtonUp;
        EventTriggerListener.Get(GrenadeBtn.gameObject).onUp    = OnButtonUp;
    }

    private void OnButtonClick(GameObject go)
    {
        int id = GetButtonId(go);
        if (id != 0)
            CBGlobalEventDispatcher.Instance.TriggerEvent<int>(ButtonEventId.ButtonClickEvent, id);
    }

    private void OnButtonDown(GameObject go)
    {
        int id = GetButtonId(go);
     //   Debug.Log("go_go:::"+ go.name + ",:::id"+id);
        if (id != 0)
            CBGlobalEventDispatcher.Instance.TriggerEvent<int>(ButtonEventId.ButtonDownEvent, id);
    }

    private void OnButtonPress(GameObject go)
    {
        int id = GetButtonId(go);
        if (id != 0)
            CBGlobalEventDispatcher.Instance.TriggerEvent<int>(ButtonEventId.ButtonPressEvent, id);
    }

    private void OnButtonUp(GameObject go)
    {
        int id = GetButtonId(go);
        if (id != 0)
            CBGlobalEventDispatcher.Instance.TriggerEvent<int>(ButtonEventId.ButtonUpEvent, id);
    }

    int GetButtonId(GameObject go)
    {
        int id = 0;

        //在这里监听按钮的点击事件
        if (go == ShootBtn.gameObject)
        {
            id = ButtonEventId.ShootButtonEventId;
        }
        else if (go == JumpBtn.gameObject)
        {
            id = ButtonEventId.JumpButtonEventId;
        }
        else if (go == GrenadeBtn.gameObject)
        {
            id = ButtonEventId.GrenadeButtonEventId;
        }
        return id;
    }

}
