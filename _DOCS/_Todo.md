## Scope
Inverse of IDisposable. Stack of Actions that will pop & Invoke on disposal.
```
using(React.Scope(out IScope scope))
    await onBtn;
```

## Message Channel
`(IPub pubMsg, ISub onMsg) = React.Channel(scope); // void channel`
`(IPub<T> pubMsg, ISub<T> onMsg) = React.Channel<T>(scope) // generic arg channel`

## Event Branch
```
(bool msg1, bool msg2) = Branch.Of(onMsg1, onMsg2)
if (msg1)
    ;
if (msg2)
    ;
```

## Routine
Custom async Task with modified flow control, without implicit schedulers, threads. Disposable. When disposed (throws internally)



### Terms & Synonims
* Reactor, Routine, Statemachine, FSM
* Message, Event
* Pub, IObserver
* Sub, Scheduler, IObservable


### Todo

* **Recipies**
  * Menu flow, Game flow back buttons, UI stack
  * Complex input scenario
  * Pools, factories
  * Button Scheduler
  * `await gizmos`, `await handles`
  * ECS reactive systems
* Advanced debugging
  * Async call stack & hierarchy: (file, method, line number)
  * Flow graph
  * Resource usage tree
* From MonoBehaviour-driven logic to message-driven hierarchical statemachines (Reactors)
  * FSM: **declarative** vs **imperative** syntax
    * async as imperative
    * async more expressive than FSM & Behaviour Trees
      * `Routine<bool>` ~ BehaviourTree
      * `Routine->Routine->Routine` ~ Hierarchical FSM
      * `Routine->Routine<bool>` ~ FSM + BehaviourTree
* React - entry point
  * Scope - inversion of IDisposable
  IDisposable 
  * Channel
    * IPub - as dried IObserver
    * ISub - as dried IObservable
* Branch
* Routine (reactor)
  * Flow
  * Schedulers
    * `Sch.TryInit`
  * await
    * `await` disposes
    * `await sub`
    * `await float`
    * GetAwaiters
  * `Routine<T>`
  * Control flow Unit Tests