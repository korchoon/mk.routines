// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Lib.DataFlow
{
    [InlineProperty, HideLabel, HideReferenceObjectPicker]
    public class SortableEdList<T>
    {
//        [Button(ButtonSizes.Large), PropertyOrder(-1)]
        public void Sort() => _sort.Invoke();

        [HideLabel, ListDrawerSettings(DraggableItems = false, IsReadOnly = true, ShowIndexLabels = false, HideRemoveButton = true, Expanded = false)]
        public List<T> All;

        Action _sort;

        public SortableEdList(Func<T, T, int> compare)
        {
            var comparison = new Comparison<T>(compare);
            All = new List<T>();
            _sort = () => All.Sort(comparison);
        }
    }
}