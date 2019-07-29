// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2017-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using JetBrains.Annotations;
using Lib.Attributes;
using Lib.DataFlow;
using Lib.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Utility.Asserts;

namespace Lib
{
    [NonPerformant(PerfKind.GC)]
    public static class SetActiveScopeApi
    {
        public static IDisposable InteractableScope(this Selectable selectable, bool interactableOnStart)
        {
//            Assert.IsNotNull(selectable);
            selectable.interactable = interactableOnStart;
//            Asr.IsTrue(selectable., $"{selectable.name}.activeInHierarchy == false");
            return new ActionOnDispose(() => selectable.interactable = !interactableOnStart);
        }

        public static string Path(GameObject g)
        {
#if UNITY_EDITOR
            return AnimationUtility.CalculateTransformPath(g.transform, null);
#else
            return string.Empty;
#endif
        }

        public static void ActivateScope(this GameObject gameObject, IScope scope)
        {
            ActivateScope(gameObject).DisposeOn(scope);
        }
        
        [MustUseReturnValue]
        public static IDisposable ActivateScope(this GameObject gameObject)
        {
            Warn.IsTrue(!gameObject.activeInHierarchy, $"{Path(gameObject)} - should be inactive before SetActive(true)");
            gameObject.SetActive(true);
            Warn.IsTrue(gameObject.activeInHierarchy, $"{Path(gameObject)} - should be active after SetActive(true)");
//            Assert.IsTrue(gameObject.activeInHierarchy, $"{Path(gameObject)}.activeInHierarchy == false");
            return new ActionOnDispose(() => gameObject.SetActive(false));
        }

        public static IDisposable ActivateComponentScope(this Behaviour comp)
        {
//            Assert.IsNotNull(comp);
            comp.enabled = true;
//            Assert.IsTrue(comp.gameObject.activeInHierarchy && comp.enabled);
            return new ActionOnDispose(() => comp.enabled = false);
        }


        public static IDisposable DeactivateScope(this GameObject gameObject)
        {
//            Assert.IsNotNull(gameObject);
            gameObject.SetActive(false); // todo msg
            return new ActionOnDispose(() => gameObject.SetActive(true));
        }

        public static void Activate(this GameObject gameObject, IScope scope)
        {
//            Assert.IsNotNull(gameObject);
            gameObject.SetActive(true);
//            Assert.IsTrue(gameObject.activeInHierarchy);
            scope.OnDispose(() => gameObject.SetActive(false));
        }

        public static void Deactivate(this GameObject gameObject, IScope scope)
        {
//            Assert.IsNotNull(gameObject);
            gameObject.SetActive(false);
            scope.OnDispose(() => gameObject.SetActive(true));
        }

        #region GameObject[]
        public static void ActivateScope(this GameObject[] gameObject, IScope scope)
        {
            gameObject.ActivateScope().DisposeOn(scope);
        }
        
        public static IDisposable ActivateScope(this GameObject[] gameObject)
        {
            Assert.IsNotNull(gameObject);
            gameObject.SetActive(true);
            return new ActionOnDispose(() => gameObject.SetActive(false));
        }

        public static IDisposable DeactivateScope(this GameObject[] gameObject)
        {
            Assert.IsNotNull(gameObject);
            gameObject.SetActive(false);
            return new ActionOnDispose(() => gameObject.SetActive(true));
        }

        public static void Activate(this GameObject[] gameObject, IScope scope)
        {
            Assert.IsNotNull(gameObject);
            gameObject.SetActive(true);
            scope.OnDispose(() => gameObject.SetActive(false));
        }

        public static void DeactivateScope(this GameObject[] gameObject, IScope scope)
        {
            Assert.IsNotNull(gameObject);
            gameObject.SetActive(false);
            scope.OnDispose(() => gameObject.SetActive(true));
        }

        public static void SetActive(this GameObject[] gameObjects, bool value)
        {
            foreach (var gameObject in gameObjects)
            {
                gameObject.SetActive(value);

                Warn.IsFalse(value && !gameObject.activeInHierarchy, $"inactive: {gameObject}");
            }
        }

        #endregion
    }
}