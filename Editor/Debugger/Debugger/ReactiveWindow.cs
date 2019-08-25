// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using Reactors;
using Reactors.Async;
using Reactors.DataFlow;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace MyNamespace
{
    public abstract class ReactiveWindow : OdinEditorWindow
    {
        IDisposable _dispose;
        protected IScope Scope;
        Ctx.Pubs _pubs;

        protected Ctx Context;

        protected override void OnEnable()
        {
            _dispose = React.Scope(out Scope);
            Context = new Ctx(out _pubs, Scope);
            Flow().GetScope(Scope);
            
            base.OnEnable();
        }

        protected abstract Routine Flow();

        protected override void OnGUI()
        {
            if (_pubs != null)
            {
                _pubs.OnGui.Next();
                var evt = Event.current;
                if (evt != null)
                {
                    _pubs.OnEvent.Next(evt);
                    ByType(evt: evt);
                }
            }

            base.OnGUI();
        }

        void ByType(Event evt)
        {
            var pubs = _pubs.EventCtx;

            switch (evt.type)
            {
                case EventType.MouseDown:
                    pubs.MouseDown.Next();
                    break;
                case EventType.MouseUp:
                    pubs.MouseUp.Next();
                    break;
                case EventType.MouseMove:
                    pubs.MouseMove.Next();
                    break;
                case EventType.MouseDrag:
                    pubs.MouseDrag.Next();
                    break;
                case EventType.KeyDown:
                    pubs.KeyDown.Next(evt.keyCode);
                    break;
                case EventType.KeyUp:
                    pubs.KeyUp.Next();
                    break;
                case EventType.ScrollWheel:
                    pubs.ScrollWheel.Next();
                    break;
                case EventType.Repaint:
                    pubs.Repaint.Next();
                    break;
                case EventType.Layout:
                    pubs.Layout.Next();
                    break;
                case EventType.DragUpdated:
                    pubs.DragUpdated.Next();
                    break;
                case EventType.DragPerform:
                    pubs.DragPerform.Next();
                    break;
                case EventType.DragExited:
                    pubs.DragExited.Next();
                    break;
                case EventType.Ignore:
                    pubs.Ignore.Next();
                    break;
                case EventType.Used:
                    pubs.Used.Next();
                    break;
                case EventType.ValidateCommand:
                    pubs.ValidateCommand.Next();
                    break;
                case EventType.ExecuteCommand:
                    pubs.ExecuteCommand.Next();
                    break;
                case EventType.ContextClick:
                    pubs.ContextClick.Next();
                    break;
                case EventType.MouseEnterWindow:
                    pubs.MouseEnterWindow.Next();
                    break;
                case EventType.MouseLeaveWindow:
                    pubs.MouseLeaveWindow.Next();
                    break;
            }
        }

        protected override void OnDestroy()
        {
            _dispose?.Dispose();
            base.OnDestroy();
        }


        public class Ctx
        {
            public EventCtx EventCtx { get; }

            public ISub OnGui { get; }
            public ISub<Event> OnEvent { get; }

            public Ctx(out Pubs pubs, IScope scope)
            {
                pubs = new Pubs();
                EventCtx = new EventCtx(out pubs.EventCtx, scope);

                (pubs.OnGui, OnGui) = scope.PubSub();
                (pubs.OnEvent, OnEvent) = scope.PubSub<Event>();
            }

            public class Pubs
            {
                public EventCtx.Pubs EventCtx;

                public IPub OnGui;
                public IPub<Event> OnEvent;
            }
        }

        public class EventCtx
        {
            public ISub MouseDown { get; }
            public ISub MouseUp { get; }
            public ISub MouseMove { get; }
            public ISub MouseDrag { get; }
            public ISub<KeyCode> KeyDown { get; }
            public ISub KeyUp { get; }
            public ISub ScrollWheel { get; }
            public ISub Repaint { get; }
            public ISub Layout { get; }
            public ISub DragUpdated { get; }
            public ISub DragPerform { get; }
            public ISub Ignore { get; }
            public ISub Used { get; }
            public ISub ValidateCommand { get; }
            public ISub ExecuteCommand { get; }
            public ISub DragExited { get; }
            public ISub ContextClick { get; }
            public ISub MouseEnterWindow { get; }
            public ISub MouseLeaveWindow { get; }

            public EventCtx(out Pubs pubs, IScope scope)
            {
                pubs = new Pubs();

                (pubs.MouseDown, MouseDown) = scope.PubSub();
                (pubs.MouseUp, MouseUp) = scope.PubSub();
                (pubs.MouseMove, MouseMove) = scope.PubSub();
                (pubs.MouseDrag, MouseDrag) = scope.PubSub();
                (pubs.KeyDown, KeyDown) = scope.PubSub<KeyCode>();
                (pubs.KeyUp, KeyUp) = scope.PubSub();
                (pubs.ScrollWheel, ScrollWheel) = scope.PubSub();
                (pubs.Repaint, Repaint) = scope.PubSub();
                (pubs.Layout, Layout) = scope.PubSub();
                (pubs.DragUpdated, DragUpdated) = scope.PubSub();
                (pubs.DragPerform, DragPerform) = scope.PubSub();
                (pubs.Ignore, Ignore) = scope.PubSub();
                (pubs.Used, Used) = scope.PubSub();
                (pubs.ValidateCommand, ValidateCommand) = scope.PubSub();
                (pubs.ExecuteCommand, ExecuteCommand) = scope.PubSub();
                (pubs.DragExited, DragExited) = scope.PubSub();
                (pubs.ContextClick, ContextClick) = scope.PubSub();
                (pubs.MouseEnterWindow, MouseEnterWindow) = scope.PubSub();
                (pubs.MouseLeaveWindow, MouseLeaveWindow) = scope.PubSub();
            }

            public class Pubs
            {
                public IPub MouseDown;
                public IPub MouseUp;
                public IPub MouseMove;
                public IPub MouseDrag;
                public IPub<KeyCode> KeyDown;
                public IPub KeyUp;
                public IPub ScrollWheel;
                public IPub Repaint;
                public IPub Layout;
                public IPub DragUpdated;
                public IPub DragPerform;
                public IPub Ignore;
                public IPub Used;
                public IPub ValidateCommand;
                public IPub ExecuteCommand;
                public IPub DragExited;
                public IPub ContextClick;
                public IPub MouseEnterWindow;
                public IPub MouseLeaveWindow;
            }
        }
    }
}