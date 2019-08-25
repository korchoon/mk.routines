// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using Reactors.Async;
using Reactors.DataFlow;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

#pragma warning disable 649

namespace Reactors.Templates
{
    [Serializable]
#if ODIN_INSPECTOR
    [InlineProperty]
#endif
    public class SceneField
    {
#if UNITY_EDITOR
#if ODIN_INSPECTOR
        [HideLabel, ShowInInspector, HorizontalGroup(Width = 0.5f, GroupName = "1"), PropertyOrder(1)]
#endif
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

        [SerializeField,]
#if ODIN_INSPECTOR
        [HideLabel, ReadOnly, HorizontalGroup(Width = 0.5f, GroupName = "1"), PropertyOrder(2)]
#endif
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

            scope.Subscribe(Dispose);

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