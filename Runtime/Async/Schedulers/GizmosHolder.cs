using System;
using Lib.Async;
using Lib.DataFlow;
using UnityEditor;
using UnityEngine;

namespace PingPong
{
    public class GizmosHolder : MonoBehaviour
    {
#if UNITY_EDITOR
        [CustomEditor(typeof(GizmosHolder))]
        class Ed : Editor
        {
            void OnSceneGUI()
            {
                var runner = (GizmosHolder) target;
                Handles.BeginGUI();
                runner._handles?.Next();
                Handles.EndGUI();
            }
        }
#endif
        IPub _handles;
        IPub _gizmo;

        public void Init(IPub pubGizmo, IPub pubHandles)
        {
            _gizmo = pubGizmo;
            _handles = pubHandles;
        }

        void OnDrawGizmos()
        {
            EdSch.IsGizmo = true;
            _gizmo?.Next();
            EdSch.IsGizmo = false;
        }
    }
}