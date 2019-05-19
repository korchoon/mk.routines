# IScope

Stores stack of `Action`'s. When disposed pops and executes them one-by-one. Has only method `void OnDispose(Action delayed)`

Make new:
`IDisposable React.Scope(out IScope)`

Produces `IDisposable` and `IScope`

## Use cases: 
* Resource cleanup. When instantiate
* Modeling hierarchical states side by side with `yield` or `await` instructions. Like `OnLeaveState`

## Recipies:
```
using(React.Scope(out IScope scope))
{
    var instance = Object.Instantiate(prefab);
    // added for automatic disposal when execution flow leaves `using`
    scope.OnDispose(() => Object.Destroy(instance.gameObject)); 
    
    // some async logic

} // instance got destroyed
```

## FAQ
**Why not `IDisposable` ?**
* You should implement `IDisposable` and not forget to call it on every 'host object' Disposal. Which leads to undisposed fields. Even IDE's now have analyzers which try to mitigate that issue but fail on non-trivial cases.
* In complex asynchronous scenarios (UI interaction) bunch of nested using statements like
// todo provide better real sample from LoTT project
```
using((IDisposable) gameMenu.BackgroundScope())  
using((IDisposable) gameMenu.PopupScope())

``` 

**OK, but why not `OnDispose(IDisposable disposable)` ?**

For the sake of simplicity. With OnDispose(Action dispose) signature you don't need wrapper class which should implements (IDisposable). Which gives you flexibility.

// 

`        public IScope PlayScope(IScope breakOn)`
