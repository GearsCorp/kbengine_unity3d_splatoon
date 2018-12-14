using CBFrame.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CBFrame.Sys
{
    public enum ArtActionState
    {
        ASS_INACTIVE,
        AAS_SUCCEED,
        ASS_FAILURE,
        AAS_BEGINE,
        AAS_RUNNING,
        AAS_END 
    }

    public abstract class CBArtAction : CBObject
    {
        protected List<object> _args;

        public GameObject Go;

        public ArtActionState State = ArtActionState.ASS_INACTIVE;

        public CBArtAction(List<object> args, GameObject obj)
        {
            _args = args;
            Go = obj;
        }

        public void Begin()
        {     
            OnBegin();
            State = ArtActionState.AAS_BEGINE;
        }

        public void End()
        {
            OnEnd();
        }

        public void Reset()
        {
            OnReset();
            State = ArtActionState.ASS_INACTIVE;
        }

        public void Update()
        {
            State = OnUpdate();
        }

        protected abstract void OnBegin();

        protected abstract void OnEnd();

        protected abstract void OnReset();

        protected abstract ArtActionState OnUpdate();
    }
}