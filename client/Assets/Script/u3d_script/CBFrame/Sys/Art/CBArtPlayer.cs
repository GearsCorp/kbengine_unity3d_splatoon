using CBFrame.Core;
using CBFrame.Utils;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CBFrame.Sys
{
    public enum ArtActionModel
    {
        AAM_SEQUENCE,
        AAM_PARALLEL
    }

    public class CBArtPlayer : CBComponent
    {
        public class Art
        {
            public Art(string artState)
            {
                ArtState = artState;
            }
            public GameObject Go;
            private List<CBArtAction> _actions = new List<CBArtAction>();
            public ArtActionModel Model = ArtActionModel.AAM_PARALLEL;

            private int _curAction;
            public  string ArtState;
            public  bool Continuity;

            private bool _bStopArt;
            private string _beginEventName;
            private List<string> _endEventLst;

            public string BeginEventName
            {
                get
                {
                    return _beginEventName;
                }

                set
                {
                    _beginEventName = value;
                }
            }
            public List<string> EndEventLst
            {
                get
                {
                    return _endEventLst;
                }

                set
                {
                    _endEventLst = value;
                }
            }
            public bool BStopArt
            {
                get
                {
                    return _bStopArt;
                }

                set
                {
                    _bStopArt = value;
                }
            }

            //串行触发
            private void SequenceModel()
            {
                if (_actions.Count == 0)
                {
                    BStopArt = true;
                    Debug.Log("_actions.count  == 0!!!");
                    return;  
                }
                if (BStopArt)
                {
                    for (int i = 0; i < _actions.Count; i++)
                    {
                        _actions[i].End();
                        Reset();
                        BStopArt = false;
                    }
                   // Debug.Log("BStopArt  == true!!!");
                    return;
                }

                if (_actions[_curAction].State == ArtActionState.AAS_END ||
                    _actions[_curAction].State == ArtActionState.ASS_FAILURE)
                {
                   // Debug.Log("endendend!!!!");
                    _actions[_curAction].End();
                    _curAction++; 
                }

                if (_curAction != _actions.Count)
                {
                   // Debug.Log("StartCoroutine_eventName222222222222222::" + _actions[_curAction].State);
                    if (_actions[_curAction].State == ArtActionState.ASS_INACTIVE)
                    {
                        _actions[_curAction].Begin();
                    }
                    _actions[_curAction].Update();
                }
                else
                {
                    for (int i = 0; i < _actions.Count; i++)
                    {
                      //  _actions[i].End();
                        Reset();
                        BStopArt = true;
                       // Debug.Log("DestotyDestory!!!!");
                    }
                }
                //for (int i = _curAction; i < _actions.Count; i++)
                //{
                //    if (_actions[i].State == ArtActionState.ASS_INACTIVE)
                //    {
                //       _actions[i].Begin();
                //    }
                //    _actions[i].Update();

                //    if (_actions[i].State == ArtActionState.AAS_SUCCEED)
                //    {
                //       _curAction++;
                //    }
                //    else if (_actions[i].State == ArtActionState.AAS_RUNNING)
                //    {
                //       break;
                //    }
                //    else if (_actions[i].State == ArtActionState.ASS_FAILURE)
                //    {
                //       Reset();
                //       return;
                //    }
                //}
          
                //if (_curAction == _actions.Count)
                //{
                //    //   Debug.Log("_curAction::"+ _curAction + ",Continuity::"+ Continuity + ",account::" + (_actions.Count));
                //    for (int i = 0; i < _actions.Count; i++)
                //    {
                //        if (!Continuity)
                //        {
                //            _actions[i].End();
                //        }
                //    }
                //    Reset();
                //    BStopArt = !Continuity;
                //    return;
                //}
            }

            //并行触发
            private void ParallelModel()
            {
                if (_actions.Count == 0)
                {
                    BStopArt = true;
                    Debug.Log("_actions.count  == 0!!!");
                    return;
                }

                if (BStopArt)
                {
                    for (int i = 0; i < _actions.Count; i++)
                    {
                        _actions[i].End();
                        Reset();
                        BStopArt = false;
                    }
                    return;
                }

                if (_curAction != _actions.Count)
                {
                    for (int i = 0; i < _actions.Count; i++)
                    {
                        if (_actions[i].State == ArtActionState.ASS_INACTIVE)
                        {
                            _actions[i].Begin();
                        }
                        _actions[i].Update();
                        _curAction++;
                    }
                }
                else
                {
                    int tmpFinishCount = 0;
                    for (int i = 0; i < _actions.Count; i++)
                    {
                        _actions[i].Update();
                        if (_actions[i].State == ArtActionState.AAS_SUCCEED ||
                         _actions[i].State == ArtActionState.ASS_FAILURE)
                        {
                            tmpFinishCount++;
                        }
                    }

                    //如果Art全部表现完成，就可以清除
                    if (tmpFinishCount == _actions.Count)
                    {
                        for (int i = 0; i < _actions.Count; i++)
                        {
                            _actions[i].End();
                            Reset();
                            BStopArt = true;
                        }
                    }

                }
            }



            public void AddEventName(string beginName, List<string> endNameLst)
            {
                BeginEventName = beginName;
                EndEventLst = endNameLst;
            }

            public void AddAction(CBArtAction action)
            {
                _actions.Add(action);
                action.Go = Go;
            }

            public void Reset()
            {
                _curAction = 0;

                for (int i = 0; i < _actions.Count; i++)
                {
                    _actions[i].Reset();
                }
            }

            public void Play()
            {
                //_curBeginCondition = _beginConditions.Count;
            }

            public void Stop()
            {
             //   _curEndCondition = _endConditions.Count;
            }

            public void Update()
            {
                //test暂时先去除掉
                return;
                if (Model == ArtActionModel.AAM_PARALLEL)
                {
                    ParallelModel();                 
                }
                else
                {
                    SequenceModel();
                }
            }
        }

        private class ConditionInfo
        {
            private string _strType;

            private Type _type;

            private List<object> _args = new List<object>();

            public ConditionInfo(string type)
            {
                string[] fragment = type.Split('|');
                _strType = fragment[0];

                if (fragment.Length == 2)
                {
                    string[] args = fragment[1].Split(',');

                    for (int i = 0; i < args.Length; i++)
                    {
                        var argContext = args[i].Split(':');

                        if (argContext.Length != 2)
                        {
                            Debug.Log("ArtPlayer: syntax error");
                            return;
                        }

                        object arg = null;

                        if (argContext[0] == "int")
                        {
                            arg = Convert.ToInt32(argContext[1]);
                        }
                        else if (argContext[0] == "bool")
                        {
                            arg = Convert.ToBoolean(argContext[1]);
                        }
                        else if (argContext[0] == "float")
                        {
                            arg = Convert.ToSingle(argContext[1]);
                        }


                        _args.Add(arg);
                    }
                }
            }

            public CBArtCondition New()
            {
                if(_type == null)
                {
                    _type = Type.GetType(_strType);

                    if(_type == null)
                    {
                        Debug.Log("ArtPlayer: unknown condition type: " + _strType);
                        return null;
                    }
                }

                return Activator.CreateInstance(_type, new object[] { _args }) as CBArtCondition;
            }
        }

        private class ActionInfo
        {
            private string _strType;

            private Type _type;

            private List<object> _args = new List<object>();

            public ActionInfo(string type)
            {
                string[] fragment = type.Split('|');
                _strType = fragment[0];

                if (fragment.Length == 2)
                {
                    string[] args = fragment[1].Split(',');

                    for (int i = 0; i < args.Length; i++)
                    {
                        var argContext = args[i].Split(':');

                        if (argContext.Length != 2)
                        {
                            Debug.Log("ArtPlayer: syntax error");
                            return;
                        }

                        object arg = null;

                        if (argContext[0] == "int")
                        {
                            arg = Convert.ToInt32(argContext[1]);
                        }
                        else if (argContext[0] == "bool")
                        {
                            arg = Convert.ToBoolean(argContext[1]);
                        }
                        else if (argContext[0] == "float")
                        {
                            arg = Convert.ToSingle(argContext[1]);
                        }
                        else
                        {
                            arg = Convert.ToString(argContext[1]);
                        }

                        _args.Add(arg);
                    }
                }
            }

            public CBArtAction New(GameObject obj)
            {
                if (_type == null)
                {
                    _type = Type.GetType(_strType);

                    if (_type == null)
                    {
                        Debug.Log("ArtPlayer: unknown Action type: " + _strType);
                        return null;
                    }
                }

                return Activator.CreateInstance(_type, new object[] { _args, obj }) as CBArtAction;
            }
        }

        public class ArtInfo
        {
            public string model;
            public List<string> actions;
            public bool continuity;

            public ArtActionModel Model;
            private List<ActionInfo> Actions = new List<ActionInfo>();


            public void Init()
            {
                for (int i = 0; i < actions.Count; i++)
                {
                    Actions.Add(new ActionInfo(actions[i]));
                }

                Model = (ArtActionModel)Enum.Parse(typeof(ArtActionModel), model);
            }

            public Art Create(string strState , GameObject obj)
            {
                Art art = new Art(strState);
                art.Go = obj;
                for (int i = 0; i < Actions.Count; i++)
                {
                    art.AddAction(Actions[i].New(obj));
                }
                art.Model = Model;
                art.Continuity = continuity;
                return art;
            }
        }


        //Avoid GC
        private List<Art> _artList = new List<Art>();

        public static List<int> ArtIDs;

        public Dictionary<string, Coroutine> RuningArtDict = new Dictionary<string, Coroutine>();


        //当前的Art
        public static string CurrentArt = "";
        public Coroutine ArtCoroutine;
        private Dictionary<string, ArtInfo> _artInfos;
        private Dictionary<string, Art> _artDict = new Dictionary<string, Art>();
        public class ArtContext
        {
           // public string currentArt { get; set; }
            public Dictionary<string, ArtInfo> ArtInfos { get; set; }
        }

        public  void Init(string path)
        {
            Debug.Log("path___path::"+ path);
            TextAsset asset = Resources.Load<TextAsset>(path);
            _artInfos = CBJsonUntity.DeserializeStringToDictionary<string, ArtInfo>(asset.text);

            // CurrentArt = context.currentArt;
            //  _artInfos  = context.ArtInfos;
            if (_artInfos == null) Debug.Log("CBArtPlayer_Init::artInfos is null!");

            foreach (var info in _artInfos)
            {
                info.Value.Init();
            }
            AddArt();
        }

        public  static  void addArtID(List<int> SelectArt)
        {
        }

        public  void AddArt()
        {
            foreach (var key in _artInfos.Keys)
            {
                Art art = _artInfos[key].Create(key, gameObject);
                _artDict.Add(key, art);
            }
        }

        public  void RemoveArt(string artDictName)
        {
        }

        public  void Play(string artDictName)
        {
            if (_artDict.ContainsKey(artDictName))
            {
                _artDict[artDictName].Play();
            }
            if (_artDict.ContainsKey(artDictName))
            {
                _artDict[artDictName].Play();
            }
        }

        public  void Reset(string artDictName)
        {
            if (_artDict.ContainsKey(artDictName))
            {
                _artDict[artDictName].Reset();
            }
        }

        private void Start()
        {
          //  Invoke("AddArt", 3);
        }

        public  void stateChanged(string eventName)
        {
            if (ArtCoroutine != null && CurrentArt != eventName)
            {
               // Debug.Log("stopBeforeCoroutine111::" + eventName + ", CurrentArt::" + CurrentArt + ", flag::"+ (ArtCoroutine == null));
                stopBeforeCoroutine(CurrentArt);
          //      Debug.Log("stopBeforeCoroutine::" + CurrentArt);
            }
            else if (ArtCoroutine != null && CurrentArt == eventName /*&& _artDict[CurrentArt].Continuity*/)
            {
                return;
            }

            if (!_artDict.ContainsKey(eventName))
            {
                Debug.LogError("Art.json no exit Art，name = " + eventName + "!");
                return;
            }

            CurrentArt = eventName;
          //  Debug.Log("StartCoroutine_eventName::" + eventName);
            ArtCoroutine = StartCoroutine(BeginArtEvent(eventName));
        }

        IEnumerator BeginArtEvent(string eventName)
        {
            
            //!开始处理Art事件
            while (!_artDict[eventName].BStopArt)
            {
                _artDict[eventName].Update();
                yield return new WaitForSeconds(0);
            }
            _artDict[eventName].BStopArt = false;
            ArtCoroutine = null;
        }

        void stopBeforeCoroutine(string eventName)
        {
            StopCoroutine(ArtCoroutine);
            _artDict[eventName].BStopArt = true;
            _artDict[eventName].Update();
            ArtCoroutine = null;
        }

    }



    #region  /// 注释掉之前的代码

    //private void SequenceModel()
    //{
    //    if (_actions.Count == 0)
    //    {
    //        return;
    //    }

    //    for (int i = _curBeginCondition; i < _beginConditions.Count; i++)
    //    {
    //        _curBeginCondition = _beginConditions[i].CheckCondition() ? _curBeginCondition + 1 : _curBeginCondition;
    //    }

    //    if (_curBeginCondition == _beginConditions.Count)
    //    {
    //        for (int i = _curAction; i < _actions.Count; i++)
    //        {
    //            if (_actions[i].State == ArtActionState.ASS_INACTIVE)
    //            {
    //                _actions[i].Begin();
    //            }

    //            _actions[i].Update();

    //            if (_actions[i].State == ArtActionState.AAS_SUCCEED)
    //            {
    //                _curAction++;
    //            }
    //            else if (_actions[i].State == ArtActionState.AAS_RUNNING)
    //            {
    //                break;
    //            }
    //            else if (_actions[i].State == ArtActionState.ASS_FAILURE)
    //            {
    //                Reset();
    //                return;
    //            }
    //        }
    //    }

    //    for (int i = _curEndCondition; i < _endConditions.Count; i++)
    //    {
    //        _curEndCondition = _endConditions[i].CheckCondition() ? _curEndCondition + 1 : _curEndCondition;
    //    }

    //    if (_curEndCondition == _endConditions.Count)
    //    {
    //        for (int i = _curAction; i < _actions.Count; i++)
    //        {
    //            _actions[i].End();
    //        }

    //        Reset();
    //    }

    //    return;
    //}

    //public void AddCondition(CBArtCondition condition, ConditionType type)
    //{
    //    condition.Go = Go;
    //    var conditions = type == ConditionType.CT_BEGIN ? _beginConditions : _endConditions;

    //    conditions.Add(condition);
    //}

    //private void ParallelModel()
    //{
    //    if (_actions.Count == 0)
    //    {
    //        return;
    //    }

    //    for (int i = _curBeginCondition; i < _beginConditions.Count; i++)
    //    {
    //        _curBeginCondition = _beginConditions[i].CheckCondition() ? _curBeginCondition + 1 : _curBeginCondition;
    //    }

    //    if (_curBeginCondition == _beginConditions.Count && _curAction != _actions.Count)
    //    {
    //        for (int i = 0; i < _actions.Count; i++)
    //        {
    //            if(_actions[i].State == ArtActionState.AAS_SUCCEED)
    //            {
    //                continue;
    //            }

    //            if (_actions[i].State == ArtActionState.ASS_INACTIVE)
    //            {
    //                _actions[i].Begin();
    //            }

    //            _actions[i].Update();

    //            if (_actions[i].State == ArtActionState.AAS_SUCCEED)
    //            {
    //                _curAction++;
    //            }
    //            else if (_actions[i].State == ArtActionState.ASS_FAILURE)
    //            {
    //                Reset();
    //                return;
    //            }
    //        }
    //    }

    //    for (int i = _curEndCondition; i < _endConditions.Count; i++)
    //    {
    //        _curBeginCondition = _endConditions[i].CheckCondition() ? _curEndCondition + 1 : _curEndCondition;
    //    }

    //    if (_curEndCondition == _endConditions.Count)
    //    {
    //        for (int i = _curAction; i < _actions.Count; i++)
    //        {
    //            _actions[i].End();
    //        }

    //        Reset();
    //    }

    //    return;
    //}

    #endregion
}