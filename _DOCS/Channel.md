# Message Channels
`IPub, ISub`
`IPub<T>, ISub<T>`

Message channel similar to IObservable (ISub), IObserver (IPub).

Subscribe
Unsubscribe
Publish

## Purpose

Decouple

## `Interfaces`
## Use cases
## Recipies (howto)
## FAQ
## Internals

## Usage
### Create

`(IPub, ISub) = React.Channel(IScope)`


`(IPub<T>, ISub<T>) = React.Channel<T>(IScope)`

### Close

disposal of Scope.

```
using(React.Scope(out IScope scope))
{
    var (pubClick, onClick) = React.Channel(scope);
    SomeUiForm(pubClick);

    await onClick;
}
```

## FAQ:
**I don't need a scope. I will not forget to Dispose it**

You will. It's the common issue with disposables. Or at least you could implement ISub, IPub and keep using the rest of library.

**