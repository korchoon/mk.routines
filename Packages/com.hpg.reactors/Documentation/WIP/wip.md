## IScope
Opposite of IDisposable
Calls in opposite direction - stack
Calls once
using React.Scope out scope
Init(IScope scope)
Instead of implementing IDisposable [and forgetting to Dispose]

##ISub
Disposes on scope
Do not send after dispose
Do not subscribe after dispose
Recipe
Future -> await
callback once
Почему нет Subject.Dispose  - т.к. IScope

##Routine
* Task cons
  * Cancellation
  * Threads lock-in
  * Confusing API

* Class vs async code block
  * **When you use class metods & fields you're missing Compiler checks for initialized value**

Exp
* multiple awaiters inside
* Multiple completes
* Breaks on dispose
* Breaks child
* Timer awaitable
* Sub awaitable
* Asserts await Task
* Recipies
* MonoBeh + Update -> Routine + while

##Branch.Of
Возвращает value tuple, который проинициализирован одним-единственным значением, остальные default. В случае ISub результат bool, для ISub<T> - Option<T>

##Recipies
* Delay input, simulate latency
* Scheduler from timer & request-replies

##Later
Async dispose: Dispose(IDisposable disposeDone)