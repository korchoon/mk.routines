// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2017-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using Lib;
using Lib.DataFlow;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;
using Utility.Asserts;

namespace Merge.View
{
    public static class EventTriggerApi
    {
        private static List<RaycastResult> _list;

        [Conditional(FLAGS.DEBUG)]
        static void CheckGraphic(Graphic g)
        {
#if TODO
            var rect = g.GetComponent<RectTransform>();
            var bound = RectTransformUtility.CalculateRelativeRectTransformBounds(g.canvas.transform);
            Debug.Log(bound);
            var pe = new PointerEventData(EventSystem.current){position = rect.pivot};
//            EventSystem.current.RaycastAll(g, _list); 
#endif
        }

        public static ISub ToSub(this Button btn, IScope sd)
        {
            Warn.IsTrue(btn.gameObject.activeInHierarchy, $"Button on {SetActiveScopeApi.Path(btn.gameObject)} - should be active");
            var (pub, sub) = React.PubSub(sd);
            var onClick = btn.onClick;
            onClick.AddListener(_Next);
            sd.OnDispose(() => onClick.RemoveListener(_Next));
            return sub;

            void _Next() => pub.Next();
        }

        [MustUseReturnValue]
        public static ISub ToSub(this EventTrigger et, EventTriggerType type, IScope sd)
        {
            var (pub, sub) = React.PubSub(sd);
            Register(et, type, _ => pub.Next(), sd);
            return sub;
        }

        [MustUseReturnValue]
        public static ISub<PointerEventData> Register(this EventTrigger et, EventTriggerType type, IScope sd)
        {
            var (pub, sub) = React.PubSub<PointerEventData>(sd);
            Register(et, type, pub.NextVoid, sd);
            return sub;
        }

        static void NextVoid<T>(this IPub<T> t, T msg) => t.Next(msg);
       
        public static void Register(this EventTrigger et, EventTriggerType type, Action<PointerEventData> callback, IScope sd)
        {
            // todo warn in couldn't raycast
            Warn.IsTrue(et.gameObject.activeInHierarchy, $"EventTrigger on {SetActiveScopeApi.Path(et.gameObject)} - should be active");

            var item = Ctor();
            et.triggers.Add(item);
            sd.OnDispose(Dispose);

            EventTrigger.Entry Ctor()
            {
                var entry = new EventTrigger.Entry()
                {
                    eventID = type,
                    callback = new EventTrigger.TriggerEvent()
                };
                entry.callback.AddListener(Callb);
                sd.OnDispose(() => entry.callback.RemoveListener(Callb));
                return entry;
            }

            void Callb(BaseEventData arg0)
            {
                if (arg0 is PointerEventData p)
                    callback.Invoke(p);
            }

            void Dispose()
            {
                et.triggers.Remove(item);
            }
        }
    }
}