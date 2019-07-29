// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2017-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using Lib.DataFlow;
using Lib.Utility;
using UnityEngine;
using Utility;
using Utility.Asserts;
using Debug = UnityEngine.Debug;

namespace Lib.Async
{
    [Serializable]
    public class ScheduleSettings
    {
        public float Logic = 0.1f;
        static float Physics
        {
            get => Time.fixedDeltaTime;
            set => Time.fixedDeltaTime = value;
        }
    }


    internal class ScheduleRunner : MonoBehaviour
    {
        static ScheduleRunner _instance;
        IPub _update;
        IPub _fixedUpdate;
        bool _completed;
        IDisposable _dispose;
        IPub<float> _fixedUpdateTime;
        IPub<float> _updateTime;
        IPub _lateUpdate;


        internal static bool TryInit(Option<ScheduleSettings> settings = default)
        {
            if (_instance) return false;

            Application.quitting += _Dispose;

            var go = new GameObject
            {
                name = "Schedulers"
            };
            _instance = go.AddComponent<ScheduleRunner>();
            _instance.transform.SetParent(null, false);
            DontDestroyOnLoad(go);

            _instance._Init(settings.ValueOr(new ScheduleSettings()));
            return true;
        }

        void _Init(ScheduleSettings scheduleSettings)
        {
            Asr.IsTrue(_instance == this);

            _dispose = React.Scope(out var scope);
            Sch.Scope = scope;
            _completed = false;

            Sch.Logic = StartSch("Logic", scheduleSettings.Logic, scope);
//            Sch.Net = StartSch("Net", settings.Net);

            Sch.Update = StartSch("Update", out _update, scope);
            Sch.LateUpdate = StartSch("LateUpdate", out _lateUpdate, scope);
            Sch.UpdateTime = StartSchTime("Update Time", out _updateTime, scope);

            Sch.Physics = StartSch("Physics", out _fixedUpdate, scope);
            Sch.PhysicsTime = StartSchTime("PhysicsTime", out _fixedUpdateTime, scope);
            (SchPub.PubError, Sch.OnError) = React.PubSub<Exception>(Sch.Scope);
        }


        void Update()
        {
            _updateTime.Next(Time.time);
            _update.Next();
        }

        void LateUpdate()
        {
            _lateUpdate.Next();
        }

        void FixedUpdate()
        {
            _fixedUpdateTime.Next(Time.fixedTime);
            _fixedUpdate.Next();
        }

        static SchBase<float> StartSchTime(string nam, out IPub<float> pub, IScope scope)
        {
            var res = new SchBase<float>(nam, out pub, scope);
            return res;
        }

        static SchBase StartSch(string nam, out IPub pub, IScope scope)
        {
            var res = new SchBase(nam, out pub, scope);
            return res;
        }

        SchBase StartSch(string nam, float time, IScope scope)
        {
            var s = new SchBase(nam, out var pub, scope);
            var yield = new WaitForSeconds(time);
            StartCoroutine(Ticker());
            return s;

            IEnumerator Ticker()
            {
                while (true)
                {
                    yield return yield;

                    pub.Next();
                }
            }
        }

        static void _Dispose()
        {
            if (_instance._completed.WasTrue()) return;

            _instance.StopAllCoroutines();
            _instance._dispose.Dispose();

            Destroy(_instance.gameObject);
            _instance = null;
        }
    }
}