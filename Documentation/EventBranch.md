# Event Branch
## Purpose
  Keep async logic dry, react differently to multiple events

## Use cases
  * Imperative
  * Callback 
  * Declarative (rx-style) 


```
(bool msg1, bool msg2) = await Branch.Of(onMsg1, onMsg2)
if (msg1)
    ;
if (msg2)
    ;
```
