using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CBFrame.Core;
using System;

namespace CBFrame.Sys
{
    public abstract class SMCondition : CBObject
    {
        internal CBStateMachine _sm;
        public SMData Data;
        Dictionary<string, int> conditionNames = new Dictionary<string, int>();
        public SMCondition(List<string> args, GameObject obj)
        {
            gameObject = obj;
            Data = new SMData();
            _sm = gameObject.GetComponent<CBStateMachine>();
            ParseConditionsData(args);

        }
        public SMCondition()
        {
        }

        public GameObject gameObject
        {
            get;
            internal set;
        }

        public void ParseConditionsData(List<string> args)
        {
            conditionNames.Clear();
            if (args.Count == 0) return;
            
            for (int i = 0; i < args.Count; i++)
            {
                var argContext = args[i].Split(':');

                if (argContext.Length != 2)
                {
                    Debug.Log("Condition_param::The length of the error!_" + args[0]);
                    return;
                }

                DataType typeID = _sm.Data.GetType(argContext[0]);
                if (DataType.DT_NONE == typeID)
                {
                    Debug.Log("default_smData is no exit " + argContext[0] + "!");
                    return;
                }
                else if (DataType.DT_INT == typeID)
                {
                    Data.AddData(argContext[0], Convert.ToInt32(argContext[1]));
                }
                else if (DataType.DT_BOOL == typeID)
                {
                    Data.AddData(argContext[0], Convert.ToBoolean(argContext[1]));
                }
                else if (DataType.DT_FLOAT == typeID)
                {
                    Data.AddData(argContext[0], Convert.ToSingle(argContext[1]));
                }
                conditionNames.Add(argContext[0], (int)typeID);
            }
        }

        public bool ifTriggerConditions()
        {
            foreach(string name in conditionNames.Keys) {
              //  Debug.Log("conditionNames[name]::" + conditionNames[name]);
                if (conditionNames[name] == (int)DataType.DT_INT)
                {
                    if (Data.GetValue<int>(name) != _sm.Data.GetValue<int>(name))
                        return false;
                }
                else if (conditionNames[name] == (int)DataType.DT_BOOL)
                {
                    if (Data.GetValue<bool>(name) != _sm.Data.GetValue<bool>(name))
                        return false;
                }
                else if (conditionNames[name] == (int)DataType.DT_FLOAT)
                {
                    if (Data.GetValue<float>(name) != _sm.Data.GetValue<float>(name))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// if the state is active, the Condititon will call once per frame.
        /// </summary>
        /// <returns></returns>
        public abstract bool Condititon();
    }
}