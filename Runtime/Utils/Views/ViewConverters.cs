using Lib;
using Lib.DataFlow;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.Proto
{
    public static class ConvertUnityEvent
    {
        public static ISub ToSub(this UnityEvent that, IScope sd) 
        {
            sd = sd ?? Empty.Scope();
            var (pub, sub) = React.Channel(sd);
            that.AddListener(ToVoid);
            sd.OnDispose(() => that.RemoveListener(ToVoid));
            return sub;

            void ToVoid() => pub.Next();
        }
    }
}