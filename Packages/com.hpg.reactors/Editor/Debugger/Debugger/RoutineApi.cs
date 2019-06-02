using Lib.Async;
using Lib.DataFlow;
using UnityEditor;
using UnityEngine;

namespace MyNamespace
{
    public static class RoutineApi
    {
        public static async Routine Button(this ISub sch, string str)
        {
            while (true)
            {
                await sch;

                if (GUILayout.Button(str))
                    break;
            }
        }
        
        public static async Routine Button(this ReactiveWindow.Ctx ctx, string str) 
        {
            while (true)
            {
                await ctx.OnGui;

                if (GUILayout.Button(str))
                    break;
            }
        }
    }
}