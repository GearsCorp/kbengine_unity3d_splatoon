using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventTriggerListener : UnityEngine.EventSystems.EventTrigger
{
    private bool m_isStartPress = false;
    private float m_curPointDownTime = 0f;
    private float m_longPressTime = 0.2f;
    private bool m_longPressTrigger = false;
    private bool m_beforePress = false;
    private GameObject _go;

    public delegate void VoidDelegate(GameObject go);
    public VoidDelegate onClick;
    public VoidDelegate onDown;
    public VoidDelegate onEnter;
    public VoidDelegate onExit;
    public VoidDelegate onUp;
    public VoidDelegate onSelect;
    public VoidDelegate onUpdateSelect;
    public VoidDelegate onPress;
    static public EventTriggerListener Get(GameObject go)
    {
        EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
        if (listener == null) listener = go.AddComponent<EventTriggerListener>();
        return listener;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (m_beforePress) return;
        if (onClick != null) onClick(gameObject);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (IsInvoking("JudgetBeforePressed"))
        {
            CancelInvoke("JudgetBeforePressed");
            m_beforePress = false;
        } 
        m_curPointDownTime = Time.time;
        m_isStartPress = true;
        m_longPressTrigger = false;
        if (onDown != null) onDown(gameObject);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (onEnter != null) onEnter(gameObject);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (IsInvoking("OnPressed")) CancelInvoke("OnPressed");
        BeforePressed();
        m_isStartPress = false;
        m_longPressTrigger = false;
        if (onExit != null) onExit(gameObject);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (IsInvoking("OnPressed")) CancelInvoke("OnPressed");
        BeforePressed();
        m_isStartPress = false;
        m_longPressTrigger = false;
        if (onUp != null) onUp(gameObject);
    }
    public override void OnSelect(BaseEventData eventData)
    {
        if (onSelect != null) onSelect(gameObject);
    }
    public override void OnUpdateSelected(BaseEventData eventData)
    {
        if (onUpdateSelect != null) onUpdateSelect(gameObject);
    }

    public  void OnPressed()
    {
        if (gameObject != null) onPress(gameObject);
    }

    public void BeforePressed()
    {
        if (m_longPressTrigger)
        {
            m_beforePress = true;
            Invoke("JudgetBeforePressed", 1f);
        }
    }

    public void JudgetBeforePressed()
    {
        m_beforePress = false;
    }

    void Update()
    {
        if (m_isStartPress && !m_longPressTrigger)
        {
            if (Time.time > m_curPointDownTime + m_longPressTime)
            {
               
                // OnPressed();
                if (m_isStartPress == true && m_longPressTrigger == false)
                {
                   // Debug.Log("rrrrrrrrrrrrrrrrrrrrrrrrr");
                    InvokeRepeating("OnPressed", 0.01f, 0.01f);
                }
                m_longPressTrigger = true;
                m_isStartPress = false;

            }
        }
    }



}
