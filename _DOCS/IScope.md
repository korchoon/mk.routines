# IScope
Provides "end of outer scope" event. Say `Dispose` which you won't forget to call.

## Purpose

Cleanup allocated resources, on exit
Undo, 
Graceful fallback on exception.
Hierarchical states

Stores stack of `Action`'s. When disposed pops and executes them one-by-one. Has only method `void OnDispose(Action delayed)`


## Interfaces


## Use cases: 
### Resource cleanup
```
var instance = Object.Instantiate(prefab);
scope.OnDispose(() => Object.Destroy(instance.gameObject)); 
```
```
LoadAssetBundle(IScope scope){
    scope.OnDispose(() => Unload())
}
```
* 
* Modeling hierarchical states side by side with `yield` or `await` instructions. Like `OnLeaveState`

## Recipies (howto)
```
void Init(IScope scope)
{
    var instance = Object.Instantiate(prefab);
    scope.OnDispose(() => Object.Destroy(instance.gameObject)); 
}
```
Make new:
`IDisposable React.Scope(out IScope)`

Produces `IDisposable` and `IScope`

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

**Why not `OnDispose(IDisposable disposable)` ?**

For the sake of simplicity. With OnDispose(Action dispose) signature you don't need wrapper class which should implements (IDisposable). Which gives you flexibility.

// 

`        public IScope PlayScope(IScope breakOn)`

## Internals

Stack of `Action`s
`IScope.OnDispose(Action)` is `stack.Push(Action)`
`Dispose` is `while(stack.Count > 0) stack.Pop().Invoke(); `