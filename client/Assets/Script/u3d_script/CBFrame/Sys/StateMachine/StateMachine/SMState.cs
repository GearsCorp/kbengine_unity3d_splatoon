using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CBFrame.Core;

namespace CBFrame.Sys
{
    public abstract class SMState : CBObject
    {
        private List<SMTransition> _transitions = new List<SMTransition>();

        private string _name;

        internal CBStateMachine _sm;

        public GameObject gameObject
        {
            get;
            private set;
        }

        public int HashCode
        {
            get;
            private set;
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                HashCode = _name.GetHashCode();
            }
        }

        public SMState(string name, GameObject go)
        {
            Name = name;
            HashCode = Name.GetHashCode();
            gameObject = go;
            _sm = gameObject.GetComponent<CBStateMachine>();
        }

        /// <summary>
        /// Make transition from origin state to dest state.
        /// </summary>
        /// <param name="dest">dest state</param>
        /// <param name="duration">translate time</param>
        /// <returns></returns>
        public SMTransition MakeTransition(string dest, float duration)
        {
            SMState state = _sm.GetState(dest);

            if(state == null)
            {
                Debug.Log("can't find state named " + dest);
                return null;
            }

            SMTransition transition = new SMTransition(this, state, duration, gameObject);
            transition._sm = _sm;       
            _transitions.Add(transition);

            return transition;
        }

        /// <summary>
        /// Remove transition.
        /// </summary>
        /// <param name="transition"></param>
        public void RemoveTransition(SMTransition transition)
        {
            _transitions.Remove(transition);
        }

        internal void Update()
        {
            OnUpdate();
            for (int i = 0; i < _transitions.Count; i++)
            {
                if (_transitions[i].CheckCondition())
                {
                    _transitions[i].Translate();
                }
            }
        }


        public void ChangeTransition()
        {
            for (int i = 0; i < _transitions.Count; i++)
            {
                if (_transitions[i].CheckCondition())
                {
                    _transitions[i].Translate();
                }
            }
        }

        public abstract void OnUpdate();
    }
}