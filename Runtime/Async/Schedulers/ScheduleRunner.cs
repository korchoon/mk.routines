using System;
using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using Lib.DataFlow;
using Lib.Utility;
using UnityEngine;
using Utility;
using Utility.AssertN;
using Debug = UnityEngine.Debug;

namespace Lib.Async
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [Serializable]
    public class ScheduleSettings
    {
        public float Logic = 0.1f;
//            public float Net = 0.05f;

        static float Physics
        {
            get => Time.fixedDeltaTime;
            set => Time.fixedDeltaTime = value;
        }
    }


    internal class ScheduleRunner : MonoBehaviour
    {
#if UNITY_EDITOR
        [CustomEditor(typeof(ScheduleRunner))]
        class Ed : Editor
        {
            void OnSceneGUI()
            {
                var runner = (ScheduleRunner) target;
                Handles.BeginGUI();
                runner._handles.Next();
                Handles.EndGUI();
            }
        }

#endif

        [Obsolete("Use instead Sch.AppScope")] public static IScope Scope => Sch.Scope;

        static ScheduleRunner _instance;
        IPub _update;
        IPub _fixedUpdate;
        CompleteToken _complete;
        IDisposable _dispose;
        IPub<float> _fixedUpdateTime;
        IPub<float> _updateTime;
        IPub _lateUpdate;
        IPub _gizmos;
        IPub _handles;


//        [Obsolete("Use instead Sch.TryInit()")]
        internal static bool TryInit(Option<ScheduleSettings> settings = default)
        {
            if (_instance) return false;

            Application.quitting += Dispose;

            var go = new GameObject
            {
//                hideFlags = HideFlags.HideAndDontSave,
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
            _complete = new CompleteToken();

            Sch.Logic = StartSch("Logic", scheduleSettings.Logic, scope);
//            Sch.Net = StartSch("Net", settings.Net);

            Sch.Update = StartSch("Update", out _update, scope);
            Sch.LateUpdate = StartSch("LateUpdate", out _lateUpdate, scope);
            Sch.UpdateTime = StartSchTime("Update Time", out _updateTime, scope);

            Sch.Physics = StartSch("Physics", out _fixedUpdate, scope);
            Sch.PhysicsTime = StartSchTime("PhysicsTime", out _fixedUpdateTime, scope);
            (SchPub.PubError, Sch.OnError) = React.Channel<Exception>(Sch.Scope);

            Init_Editor(scope);
            Init_Player(scope);
        }

        [Conditional(FLAGS.NON_EDITOR)]
        void Init_Player(IScope _)
        {
            EdSch.Gizmos = Empty.Sub();
            _gizmos = Empty.Pub();

            EdSch.Handles = Empty.Sub();
            _handles = Empty.Pub();
        }

        [Conditional(FLAGS.UNITY_EDITOR)]
        void Init_Editor(IScope scope)
        {
            // runtime
            EdSch.Handles = StartSch("Handles", out _handles, scope);
            EdSch.Gizmos = StartSch("Gizmos", out _gizmos, scope);
        }

        public static bool WantsQuit { get; private set; }

        void OnDrawGizmos() => _gizmos.Next();

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

        SchBase<float> StartSchTime(string nam, out IPub<float> pub, IScope scope)
        {
            var res = new SchBase<float>(nam, out pub, scope);
            return res;
        }

        SchBase StartSch(string nam, out IPub pub, IScope scope)
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

        static void Dispose()
        {
            if (_instance._complete.Set()) return;

            _instance.StopAllCoroutines();

#if !BUG_INF_LOOP_FIXED
            _instance._dispose.Dispose();
#endif
            Destroy(_instance.gameObject);
            _instance = null;
        }
    }
}