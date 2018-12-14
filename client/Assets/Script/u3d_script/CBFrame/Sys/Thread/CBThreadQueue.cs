using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;
using CBFrame.Utils;

namespace CBFrame.Thread
{
    public class CBThreadQueue : CBMonoSingleton<CBThreadQueue>
    {
        private static int _numThreads;

        public static int _maxThreads = 4;

        private List<Action> _actions = new List<Action>();

        private List<Action> _asyncActions = new List<Action>();

        private List<Action> _currentActions = new List<Action>();

        public struct DelayedQueueItem
        {
            public float time;
            public Action action;
        }

        private List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();

        private List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();

        protected override void OnInit()
        {
            base.OnInit();
            ThreadPool.SetMaxThreads(6, 6);
            ThreadPool.SetMinThreads(4, 4);
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            lock (_actions)
            {
                _currentActions.Clear();
                _currentActions.AddRange(_actions);
                _actions.Clear();
            }

            foreach (var action in _currentActions)
            {
                action();
            }

            lock (_delayed)
            {
                _currentDelayed.Clear();
                _currentDelayed.AddRange(_delayed.Where(d => d.time <= Time.time));

                foreach (var item in _currentDelayed)
                {
                    _delayed.Remove(item);
                }
            }

            foreach (var delayed in _currentDelayed)
            {
                delayed.action();
            }

            for (int i = 0; i < _asyncActions.Count; i++)
            {
                if (!ThreadPool.QueueUserWorkItem(RunAction, _asyncActions[i]))
                {
                    break;
                }
            }
        }

        private static void RunAction(object action)
        {
            try
            {
                ((Action)action)();
            }
            catch
            {
            }
            finally
            {
                //Interlocked.Decrement(ref _numThreads);
            }
        }

        public static void RunOnMainThread(Action action)
        {
            RunOnMainThread(action, 0f);
        }

        public static void RunOnMainThread(Action action, float time)
        {
            if (time != 0)
            {
                lock (Instance._delayed)
                {
                    Instance._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action });
                }
            }
            else
            {
                lock (Instance._actions)
                {
                    Instance._actions.Add(action);
                }
            }
        }

        public static void RunAsync(Action a)
        {
            ThreadPool.QueueUserWorkItem(RunAction, a);
        }

        public new void StartCoroutine(IEnumerator routine)
        {
            CBThreadQueue.RunOnMainThread(() =>
            {
                base.StartCoroutine(routine);
            });
        }
    }
}