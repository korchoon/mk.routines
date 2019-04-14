using System;
using System.IO;
using System.Linq;
using Lib.Async;
using Lib.DataFlow;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

#pragma warning disable 649

namespace Lib.Templates
{
    [Serializable, InlineProperty]
    public class SceneField
    {
#if UNITY_EDITOR
        [HideLabel, ShowInInspector, HorizontalGroup(Width = 0.5f, GroupName = "1"), PropertyOrder(1)]
        Object _sceneAsset2
        {
            get =>
                (from scene in UnityEditor.EditorBuildSettings.scenes
                    let filename = Path.GetFileNameWithoutExtension(scene.path)
                    where scene.enabled && StringComparer.OrdinalIgnoreCase.Equals(filename, _sceneName) 
                    select UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.SceneAsset>(scene.path))
                .FirstOrDefault();

            set => _sceneName = value?.name;
        }

#endif


        [SerializeField, HideLabel, ReadOnly, HorizontalGroup(Width = 0.5f, GroupName = "1"), PropertyOrder(2)]
        string _sceneName;

        // for LoadLevel/LoadScene
        public static implicit operator string(SceneField sceneField)
        {
            return sceneField._sceneName;
        }

        public async Routine Load(IScope scope, bool activate = false)
            //, INext<AsyncOperation> onUnload = default
        {
            var previous = SceneManager.GetActiveScene();
            var t = SceneManager.LoadSceneAsync(this, LoadSceneMode.Additive);
            while (!t.isDone) await Sch.Update;

            scope.OnDispose(Dispose);

            if (!activate)
                return;

            SceneManager.sceneUnloaded += ActivatePrevious;
            var sceneAt = SceneManager.GetSceneByName(_sceneName);
            SceneManager.SetActiveScene(sceneAt);
            return;

            void ActivatePrevious(Scene _)
            {
                SceneManager.sceneUnloaded -= ActivatePrevious;
                SceneManager.SetActiveScene(previous);
            }

            void Dispose()
            {
                var res = SceneManager.UnloadSceneAsync(this);
            }
        }
    }
}