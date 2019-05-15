# Routine

 Синтаксис позволяет емко писать асинхронную логику, наиболее приближенно к естественному языку. По поведению напоминает иерархические конечные автоматы. Каждую Routine можно воспринимать как "состояние" конечного автомата.

## async Routine vs async Task
| Задача     | `Coroutine (IEnumerator)`   | `async Routine`  |
| :-----------------------: |:-------------| :-----:|
| Моментальная остановка запущенного Task | завести CancellationTokenSource, передать CancellationTokenSource, проверять после каждого await <IsCompleted> | .Dispose() |
| Остановка Task и вложенных | Нет | Есть |
| Многопоточность | Есть | Нет |

## IObservable vs ISub vs event/delegate
| Задача     | IObservable | ISub | event |
| :-----------------------: |:-------------| :---:| :-----:|
| Подписка |  |  |
| Отписка |  |  |
| Подписка на 1 раз |  |  |
|  |  |  |
|  |  |  |


## async vs IEnumerator
| Задача     | `Coroutine (IEnumerator)`   | `async Routine`  |
| :-----------------------: |:-------------| :-----:|
| Объявление блока | IEnumerator | async Routine |
| Вернуть управление | yield return | await |
| Запуск | StartCoroutine(IEnumerator) | вызов метода |
| Остановка | StopCoroutine, MB.enabled = false | Routine.Dispose(), throw Exception |
|  |  |  |


Похож на IEnumerator/yield return
async Routine ~ IEnumerator
await ~ yield return
return ~ yield break

### Стейтмашина, поверхностное описание
Для обеих компилятор генерирует конечный автомат. По инструкциям <возврата контекста> yield/await, код бьется на блоки = кол-во инструкций + 1. По ним генерируются классы с методом bool MoveNext(). Поля - переменные в блоке. MoveNext возвращает новый класс, выполняет инструкции в блоке, меняет внутреннее состояние.
Подробнее - Dixin's blog, decompiler.

Возврат значения
IEnumerator возвращает серию значений, получаемую через .Current
async - одно значение Routine.GetAwaiter.GetResult / GetException

MoveNext
IEnumerator - public bool MoveNext - т.е. требует внешний шедулер
async Routine - правила MoveNext определяются очередным await. Т.е. наоборот

Задачи:
Подождать что-то
yield return SomeEnumerator();
await SomeRoutine();

подождать что-то и получить значение
-- не предназначено для этого. 
var result = await SomeRoutine();

| Задача     | `StartCoroutine(IEnumerator)`   | `async Routine`  |
| :-----------------------: |:-------------| :-----:|
| Подождать событие, вернуть значение|  | |
| Подождать, вернуть значение |  |  |
| Прервать запущенную |  |  |
|  |  |  |

Основное отличие от блоков IEnumerator: IEnumerator используется как правило для ленивой генерации последовательности нескольких объектов (поле Current), наружу выдает (bool MoveNext) для того, чтобы какой-то шедулер продвигал вперед (например каждый кадр через StartCoroutine).
async - шедулер спрятан внутри, неявный. В случае async Task - это шедулер на основе ThreadPool. В случае Routine 

 async дает понять компилятору, что метод надо преобразовать в стейт-машину: набор классов = кол-ву блоков между await.  

 ## async/await flow

async помечает


 Вся мощь проявляется в сочетании с PubSub (IObservable, IObserver), и Scope (обеспечивает действия при выходе за область видимости)



# Routine

Short natural notation of asynchronous logic. Could be used to make Hierarchical Finite State Machines and Behaviour Trees. Grants graceful fallback to upper state in case of exception. 
Especially useful


Example:
Seek
Hide
Shoot
Run
Alive

## Purpose


## `Interfaces`
## Use cases
## Recipies (howto)
## FAQ
## Internals

# Routine

`Brief desc
Built on top of Pub,Sub
Purpose
imperative hierarchical state-machines

## Why not _ ?
Comparing to `Task`:
* Disposable (throws)
* Explicit schedulers (ISub)
* 

## Use cases:
* Hierarchical state machines
* Behaviour trees: `Routine<bool>`


## Details
### Disposal
throws exception internally to 

### Awaiters