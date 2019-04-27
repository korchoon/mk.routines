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