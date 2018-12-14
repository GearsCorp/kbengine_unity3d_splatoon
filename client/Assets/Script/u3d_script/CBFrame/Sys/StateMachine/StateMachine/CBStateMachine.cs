using CBFrame.Core;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System;

namespace CBFrame.Sys
{

    public class CBStateMachine : CBComponent
    {
        public class ParamContext
        {
            public string fileName { get; set; }
            public List<string> paramDatas { get; set; }
        }
        public class TranContext
        {
            public string origin { get; set; }
            public string dest { get; set; }
            public float duration { get; set; }
            public List<ParamContext> conditions { get; set; }
        }

        public class SMContext
        {
            public List<string> states { get; set; }
            public string default_state { get; set; }
            public List<string>  default_params { get; set; }
            public List<TranContext> transitions { get; set; }
        }

        private SMState _activeState;

        public SMData Data = new SMData();

        public Dictionary<int, SMState> _statesByHash = new Dictionary<int, SMState>();

        public Dictionary<string, SMState> _statesByName = new Dictionary<string, SMState>();

        public TextAsset SMFile;

        public string SMFilePath = "";

        /// <summary>
        /// Defalut state
        /// </summary>
        public SMState Default
        {
            get;
            set;
        }

        /// <summary>
        /// 当前活动状态
        /// </summary>
        public SMState ActiveState
        {
            get
            {
                return _activeState;
            }
            private set
            {
                if (_activeState != value)
                {
                    var old = _activeState;
                    _activeState = value;
                    //TODO: SendMessage;
                  //  Debug.Log(string.Format("State from {0} to {1}", old, _activeState));
                }
            }
        }

        /// <summary>
        /// change the state
        /// </summary>
        /// <param name="state">
        /// the state for change. it's internal func.
        /// </param>
        internal void ChangeState(SMState state)
        {
            var old = ActiveState;
            ActiveState = state;
        }

        /// <summary>
        /// load state from file.
        /// </summary>
        /// <param name="asset">
        /// the state machine file.
        /// </param>
        /// <returns></returns>
        private bool LoadFromFile(TextAsset asset)
        {
            SMContext context = JsonConvert.DeserializeObject<SMContext>(asset.text);

            //!解析状态值
            for (int i = 0; i < context.states.Count; i++)
            {
                try
                {
                    SMState state = Activator.CreateInstance(Type.GetType(context.states[i]),
                                                             new object[] { context.states[i], gameObject }) as SMState;
                    _statesByName.Add(context.states[i], state);
                    _statesByHash.Add(state.HashCode, state);
                }
                catch
                {
                    Debug.Log("can't create state named " + context.states[i]);
                    continue;
                }
            }

            Default = GetState(context.default_state);
            _activeState = Default;

            //!解析默认参数值
            foreach (string param in context.default_params)
            {
                ParseParamData(param);
            }

            //!解析transtitions
            for (int i = 0; i < context.transitions.Count; i++)
            {
                TranContext tranContext = context.transitions[i];
                SMState origin = GetState(tranContext.origin);

                if (origin == null)
                {
                    Debug.Log(string.Format("can't make transition from {0} to {1}",
                                            tranContext.origin, tranContext.dest));
                    continue;
                }

                SMTransition transition = origin.MakeTransition(tranContext.dest, tranContext.duration);

                if (transition == null)
                {
                    Debug.Log(string.Format("can't make transition from {0} to {1}",
                                            tranContext.origin, tranContext.dest));
                    continue;
                }

                for (int j = 0; j < context.transitions[i].conditions.Count; j++)
                {
                    string fileName = tranContext.conditions[j].fileName;
                    List<string> args = tranContext.conditions[j].paramDatas;

                    try
                    {
                        SMCondition condition = Activator.CreateInstance(Type.GetType(fileName), new object[] {args, gameObject}) as SMCondition;
                        transition.AddCondition(condition);
                    }
                    catch
                    {
                        Debug.Log(string.Format("can't make transition from {0} to {1}, condition {2} not found!",
                                            tranContext.origin, tranContext.dest, tranContext.conditions[j]));
                        origin.RemoveTransition(transition);
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// load state from file.
        /// </summary>
        /// <param name="path">
        /// file path without extension.
        /// </param>
        /// <returns></returns>
        public bool LoadFromFile(string path)
        {
           // string localPath = Application.dataPath + "/" + path;
            Debug.Log("localPath:::" + path);
            TextAsset jsonStr = (TextAsset)Resources.Load(path);
            if (jsonStr == null)
            {
                Debug.Log("jsonStr_jsonStr!!");
                return false;
            }
            LoadFromFile(jsonStr);
            Debug.Log(jsonStr.text);
            return true;
        }

        /// <summary>
        /// add a new state
        /// </summary>
        /// <param name="state">
        /// 
        /// </param>
        /// <returns></returns>
        public bool AddState(SMState state)
        {
            if (_statesByHash.ContainsKey(state.HashCode))
            {
                return false;
            }

            state._sm = this;
            _statesByHash.Add(state.HashCode, state);

            return true;
        }

        /// <summary>
        /// Remove state by hash code.
        /// </summary>
        /// <param name="hashCode"></param>
        public void RemoveState(int hashCode)
        {
            if (_statesByHash.ContainsKey(hashCode))
            {
                SMState state = _statesByHash[hashCode];

                _statesByHash.Remove(hashCode);
                _statesByName.Remove(state.Name);
            }
        }

        /// <summary>
        /// Remove state by name.
        /// </summary>
        /// <param name="name"></param>
        public void RemoveState(string name)
        {
            _statesByHash.Remove(name.GetHashCode());
            _statesByName.Remove(name);
        }

        /// <summary>
        /// add a int parameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddParameter(string name, int value)
        {
            Data.AddData(name, value);
          //  TransitionParameter();
        }

        public void AddParameter(string name, bool value)
        {
            Data.AddData(name, value);
           // TransitionParameter();
        }

        public void AddParameter(string name, float value)
        {
            Data.AddData(name, value);
          //  TransitionParameter();
        }

        public void SetParameter(string name, int value)
        {
            Data.SetParameter(name, value);
            TransitionParameter();
            UpdateState();
        }
        public void SetParameter(string name, bool value)
        {
            Data.SetParameter(name, value);
            TransitionParameter();
            UpdateState();
        }

        public void SetParameter(string name, float value)
        {
            Data.SetParameter(name, value);
            TransitionParameter();
            UpdateState();
        }

        public void TransitionParameter()
        {
            _activeState.ChangeTransition();
        }

        public void UpdateState()
        {
            if (ActiveState != null)
            {
                ActiveState.Update();
            }
        }
        /// <summary>
        /// change the state
        /// </summary>
        /// <param name="name">
        /// state name
        /// </param>
        /// <param name="duration">
        /// translate time.
        /// </param>
        public void ChangeState(string name, float duration)
        {
            if (_statesByName.ContainsKey(name))
            {
                ActiveState = _statesByName[name];

                if (ActiveState == null)
                {
                    Debug.Log("ChangeState_name::" + name + ",,,,_statesByName:::" + _statesByName);
                }
            }
        }

        /// <summary>
        /// get state by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SMState GetState(string name)
        {
            if (name == null)
            {
                return null;
            }

            SMState state = null;

            if (_statesByName.ContainsKey(name))
            {
                state = _statesByName[name];
            }

            return state;
        }

        // Update is called once per frame
        //public override void OnRenderStart()
        //{
        //    if (ActiveState != null)
        //    {
        //        ActiveState.Update();
        //    }
        //}

        public void ParseConditionsData(string strCondition, out string fileName, ref List<string> args)
        {
            string[] fragment = strCondition.Split('|');
            fileName = fragment[0];

            if (fragment.Length == 2)
            {
                string[] tmpArgs = fragment[0].Split(',');
                foreach(string arg in tmpArgs)
                {
                    args.Add(arg);
                }
            }
        }

        public void ParseParamData(string param)
        {
            string[] args = param.Split('|');
            if (args.Length != 3) {
                Debug.Log("ParseParamsData:" + param + "is not suit!");
                return;
            }

            if (args[1] == "int" || args[1] == "Int")
            {
                Data.AddData(args[0], Convert.ToInt32(args[2]));
            }
            else if (args[1] == "bool" || args[1] == "Bool")
            {
                Data.AddData(args[0], Convert.ToBoolean(args[2]));
            }
            else if (args[1] == "float" || args[1] == "Float")
            {
                Data.AddData(args[0], Convert.ToSingle(args[2]));
            }
        }
    }
}