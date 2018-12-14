using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CBFrame.Sys
{
    public enum DataType {
        DT_NONE = 0,
        DT_INT,
        DT_BOOL,
        DT_FLOAT
    }

    public class SMData
    {
        public Dictionary<string, int> smDataInt = new Dictionary<string, int>();

        public Dictionary<string, bool> smDataBool = new Dictionary<string, bool>();

        public Dictionary<string, float> smDataFloat = new Dictionary<string, float>();

        public void AddData(string name, int value)
        {
            if (GetType(name) == DataType.DT_NONE)
            {
                smDataInt.Add(name, value);
            }
            else {
                Debug.Log("parameter named " + name + "already exists!!!");
            }   
        }

        public void AddData(string name, bool value)
        {
          //  Debug.Log("parameter named:: " + name + ",value::"+ value);
            if (GetType(name) == DataType.DT_NONE)
            {
                smDataBool.Add(name, value);
            }
            else
            {
                Debug.Log("parameter named " + name + "already exists!!!");
            }
        }

        public void AddData(string name, float value)
        {
            if (GetType(name) == DataType.DT_NONE)
            {
                smDataFloat.Add(name, value);
            }
            else
            {
                Debug.Log("parameter named " + name + "already exists!!!");
            }
        }

        public void SetParameter(string name, float value)
        {
            if(GetType(name) != DataType.DT_NONE)
            {
                smDataFloat[name] = value;
            }
        }

        public void SetParameter(string name, bool value)
        {
            if (GetType(name) != DataType.DT_NONE)
            {
                smDataBool[name] = value;
            }
        }

        public void SetParameter(string name, int value)
        {
            if (GetType(name) != DataType.DT_NONE)
            {
                smDataInt[name] = value;
            }
        }
        public DataType GetType(string name)
        {
            if (smDataInt.ContainsKey(name))
            {
                return DataType.DT_INT;
            }
            else if (smDataBool.ContainsKey(name))
            {
                return DataType.DT_BOOL;
            }
            else if (smDataFloat.ContainsKey(name))
            {
                return DataType.DT_FLOAT;
            }
            return DataType.DT_NONE;
        }

        //public int GetValue(string name, int dataType)
        //{
        //    return smDataInt[name];
        //}

        //public float GetValue(string name, float dataType)
        //{
        //    return smDataFloat[name];
        //}

        //public bool GetValue(string name, bool dataType)
        //{
        //    return smDataBool[name];
        //}

        public T GetValue<T>(string name)
        {
            object value = null;
            if (smDataInt.ContainsKey(name))
            {
                value = smDataInt[name];
                return (T)value;
            }
            else if (smDataFloat.ContainsKey(name))
            {
                value = smDataFloat[name];
                return (T)value;
            }
            else if (smDataBool.ContainsKey(name))
            {
                value = smDataBool[name];
                return (T)value;
            }
            else return (T)value;
        }
    }
}