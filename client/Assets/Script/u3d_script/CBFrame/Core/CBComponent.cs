using CBFrame.Sys;
using KBEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CBFrame.Core
{
    [RequireComponent(typeof(CBGoEventDispatcher))]
    public class CBComponent : FrameSyncBehaviour
    {
        private CBGoEventDispatcher _dispatcher;

        private void Awake()
        {
            _dispatcher = GetComponent<CBGoEventDispatcher>();
        }

        /// <summary>
        ///  增加监听器， 不带参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener(int eventType, Action handler)
        {
            if(_dispatcher)
                _dispatcher.AddEventListener(eventType, handler);
        }

        public void AddEventListener<T>(int eventType, Action<T> handler)
        {
            if (GetComponent<CBGoEventDispatcher>() == null) Debug.Log("sssssssssssssssssssss:::::"+ _dispatcher);
            if (_dispatcher)
                _dispatcher.AddEventListener(eventType, handler);
        }

        public void AddEventListener<T, U>(int eventType, Action<T, U> handler)
        {
            if (_dispatcher)
                _dispatcher.AddEventListener(eventType, handler);
        }

        public void RemoveEventListener(int eventType, Action handler)
        {
            if (_dispatcher)
                _dispatcher.RemoveEventListener(eventType, handler);
        }

        public void RemoveEventListener<T>(int eventType, Action<T> handler)
        {
            if (_dispatcher)
                _dispatcher.RemoveEventListener(eventType, handler);
        }

        public void TriggerEvent(int eventType)
        {
            if (_dispatcher)
                _dispatcher.TriggerEvent(eventType);
        }

        public void TriggerEvent<T>(int eventType, T arg1)
        {
         //   Debug.Log("_dispatcher::" + _dispatcher + ",eventType::"+ eventType);
            if (_dispatcher)
                _dispatcher.TriggerEvent(eventType, arg1);
            else
            {
                Debug.Log("_dispatcher is null!!!");
            }
        }

        /// <summary>
        ///  触发事件， 带2个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        public void TriggerEvent<T, U>(int eventType, T arg1, U arg2)
        {
            if (_dispatcher)
                _dispatcher.TriggerEvent(eventType, arg1, arg2);
        }

        /// <summary>
        ///  触发事件， 带3个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        public void TriggerEvent<T, U, V>(int eventType, T arg1, U arg2, V arg3)
        {
            if (_dispatcher)
                _dispatcher.TriggerEvent(eventType, arg1, arg2, arg3);
        }

        /// <summary>
        ///  触发事件， 带4个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        public void TriggerEvent<T, U, V, W>(int eventType, T arg1, U arg2, V arg3, W arg4)
        {
            if (_dispatcher)
                _dispatcher.TriggerEvent(eventType, arg1, arg2, arg3, arg4);
        }

        private void OnDestroy()
        {
            if (_dispatcher)
                _dispatcher.RemoveEventListener(this);
        }


     
    }
}