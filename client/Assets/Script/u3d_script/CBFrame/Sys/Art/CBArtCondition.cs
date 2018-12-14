using CBFrame.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CBFrame.Sys
{
    public struct ArtData
    {
        public string Key;
        public object Param;
    }

    public abstract class CBArtCondition : CBObject
    {
        protected List<object> _params = new List<object>();

        public GameObject Go;

        public CBArtCondition(List<object> args)
        {
            _params = args;
        }

        public abstract bool CheckCondition();

        public abstract void OnReset();
    }
}