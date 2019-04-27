using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Lib.DataFlow
{
    [InlineProperty, HideLabel, HideReferenceObjectPicker]
    public class SortableEdList<T>
    {
        [Button(ButtonSizes.Large), PropertyOrder(-1)]
        void Sort() => _sort.Invoke();

        [ListDrawerSettings(DraggableItems = false, IsReadOnly = true, ShowIndexLabels = false, HideRemoveButton = true, Expanded = false)]
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