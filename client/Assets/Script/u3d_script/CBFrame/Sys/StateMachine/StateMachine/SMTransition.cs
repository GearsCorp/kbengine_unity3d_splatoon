using CBFrame.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CBFrame.Sys
{
    public class SMTransition : CBObject
    {
        internal CBStateMachine _sm;

        private List<SMCondition> _conditions = new List<SMCondition>();

        public SMState Origin
        {
            get;
            internal set;
        }

        public SMState Dest
        {
            get;
            private set;
        }

        public GameObject gameObject
        {
            get;
            private set;
        }

        public float Duration
        {
            get;
            set;
        }

        public SMTransition(SMState origin, SMState dest, float duration, GameObject go)
        {
            gameObject = go;
            Dest = dest;
            Duration = duration;
            _sm = gameObject.GetComponent<CBStateMachine>();
        }

        public void AddCondition(SMCondition condition)
        {
            _conditions.Add(condition);
        }

        public void RemoveCondition(int index)
        {
            _conditions.RemoveAt(index);
        }

        public void Translate(bool bImmediate = false)
        {

            if(!bImmediate && Duration > 0)
            {
                //TODO: 
                _sm.StartCoroutine(TranslateTo(Duration));
                return;
            }

            _sm.ChangeState(Dest);
        }

        //internal bool CheckCondition()
        //{
        //    for(int i = 0; i< _conditions.Count; i++)
        //    {
        //        if(!_conditions[i].Condititon())
        //        {
        //            return false;
        //        }
        //        ////#新增加功能：这里是只要一个总的condition成立，就可以调整返回
        //        //return true;
        //    }

        //    return true;
        //}

        //#新增加功能：这里是只要一个总的condition成立，就可以调整返回
        internal bool CheckCondition()
        {
            for (int i = 0; i < _conditions.Count; i++)
            {
                if (_conditions[i].Condititon())
                {
                    return true;
                }
            }

            return false;
        }

        private IEnumerator TranslateTo(float duration)
        {
            yield return new WaitForSeconds(duration);
            _sm.ChangeState(Dest);
        }
    }
}