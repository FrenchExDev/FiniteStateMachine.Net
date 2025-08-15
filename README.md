# `FrenchExDev.Net.FiniteStateMachine.Core`

A `tiny` & `flexible`, `enum-typed` `generic` `Finite State Machine` `library` for `.NET`, written in `C#`.

This `library` enables you to `separate` `modeling` and `building` of `executable stateful machines`, with support for `custom actions`, `conditions`, and `transition-specific behaviors`.

---

## Features

- **`Strongly-typed FSMs`** Use your own types for states, triggers, and context objects.
- **`Builder Pattern`** Fluent API for defining states, transitions, and actions.
- **`Transition Actions & Conditions`** Attach logic and guards to transitions.
- **`State Entry Actions`** Execute logic when entering a state.
- **`Unit-tested`** Includes comprehensive tests using a typical FSM scenario.

---

## Getting Started

### 1. Install

```powershell
dotnet add package FrenchExDev.Net.FiniteStateMachine.Core
```

### 2. Define your Class

Create a class that will represent the context of your state machine:

```csharp
class Door
{
    public bool Locked { get; set; }
    public void History(DeviceState state, DeviceEvent trigger, DateTime timestamp){ /* ... */ }
    public void ResetAvailableOperations(DeviceOperation[] operations) { /* ... */ }
}
```

### 3. Define Your Events and States

Define enums or classes for your states and triggers:

```csharp
enum DoorState { Closed, Open, Locked }
enum DoorEvent { Open, Close, Lock, Unlock }
```

### 4. Build a State Machine

Use the builder to define states and transitions:

```csharp
using FrenchExDev.Net.FiniteStateMachine.Core;

var builder = new FiniteStateMachineBuilder<Door, DoorState, DoorEvent>();

builder
    .Transition(fromState: DoorState.Closed, toState: DoorState.Open, on: DoorEvent.Open)
    .Transition(fromState: DoorState.Open, toState: DoorState.Closed, on: DoorEvent.Close)
    .Transition(fromState: DoorState.Closed, toState: DoorState.Locked, on: DoorEvent.Lock, body: (door, e, fsm) => {
        door.Locked = true;
    })
    .Transition(fromState: DoorState.Locked, toState: DoorState.Closed, on: DoorEvent.Unlock, body: (door, e, fsm) =>
    {
        door.Locked = false;
    });

var door = new Door();
var fsm = builder.Build(door, DoorState.Closed);
```

### 5. Fire triggers

```csharp
fsm.Fire(DoorEvent.Open);   // Door transitions to Open, returned TransitionResult.Success
fsm.Fire(DoorEvent.Close);  // Door transitions to Closed, returned TransitionResult.Success
fsm.Fire(DoorEvent.Lock);   // Door transitions to Locked, sets door.Locked = true, returned TransitionResult.Success
fsm.Fire(DoorEvent.Unlock); // Door transitions to Closed, sets door.Locked = false, returned TransitionResult.Success
```
_____

# Advanced: State Entry Actions

imagine the following scenarios:
* You have a device with Available Operations and History
* Your have its State and Event enums

```csharp
using FrenchExDev.Net.FiniteStateMachine.Core;

public class Device {
    public Device History(DeviceState state, DeviceEvent trigger, DateTime timestamp) { /* ... */ }
    public Device ResetAvailableOperations(DeviceOperation[] operations) { /* ... */ }
}

public enum DeviceState { NotInitialized, Initing, Available, Connected }
public enum DeviceEvent { Init, Inited, Connect }

```

You can use the `WhenFiniteStateMachineBuilder` to define `actions` that run 

* when `a specific event` is fired using `.On(DeviceEvent trigger, Action<Device, DeviceEvent, WhenFiniteStateMachine<Device, DeviceState, DeviceEvent>> action);`
* whenever `a specific state` is entered, regardless of the trigger using `.When(DeviceState state, Action<Device, DeviceEvent, WhenFiniteStateMachine<Device, DeviceState, DeviceEvent>> action).);`

```csharp
using FrenchExDev.Net.FiniteStateMachine.Core;

var builder = new WhenFiniteStateMachineBuilder<Device, DeviceState, DeviceEvent>();

builder
    .On(DeviceEvent.Open, (device, trigger, fsm) =>     {
        device.History(DeviceState.Open, trigger, DateTime.UtcNow);
    })
    .When(DeviceState.Connected, (device, trigger, fsm) =>
    {
        device.ResetAvailableOperations(new[] { DeviceOperation.Disconnect, DeviceOperation.Upload });
    });

builder
    .Transition(on: DeviceEvent.Init, fromState: DeviceState.NotInitialized, toState: DeviceState.Initing, body: (device, e, fsm) =>
    {
        device.History(DeviceState.Initing, e, DateTime.UtcNow);
        fsm.Fire(DeviceEvent.Inited);
    })
    .Transition(on: DeviceEvent.Inited, fromState: DeviceState.Initing, toState: DeviceState.Available, body: (device, e, fsm) =>
    {
        device.History(DeviceState.Available, e, DateTime.UtcNow);
    })
    .Transition(on: DeviceEvent.Connect, fromState: DeviceState.Available, toState: DeviceState.Connected, body: (device, e, fsm) =>
    {
        device.History(DeviceState.Connected, e, DateTime.UtcNow);
    });

var device = new Device();
var fsm = builder.Build(device, DeviceState.NotInitialized);

fsm.Fire(DeviceEvent.Init);    // Device transitions to Initing, then to Available, returned TransitionResult.Success
fsm.Fire(DeviceEvent.Connect); // Device transitions to Connected, returned TransitionResult.Success
```

# Running Tests

* [Device Test](https://github.com/FrenchExDev/FrenchExDev.FiniteStateMachine.Net/blob/main/test/FrenchExDev.Net.FiniteStateMachine.Core.Tests/DeviceFiniteStateMachineTests.cs)
* [Door Test](https://github.com/FrenchExDev/FrenchExDev.FiniteStateMachine.Net/blob/main/test/FrenchExDev.Net.FiniteStateMachine.Core.Tests/FiniteStateMachineTests.cs)
* [WhenFiniteStateMachine Test](https://github.com/FrenchExDev/FrenchExDev.FiniteStateMachine.Net/blob/main/test/FrenchExDev.Net.FiniteStateMachine.Core.Tests/DeviceWhenFiniteStateMachineTests.cs)

Open a `Terminal` in the `.sln` directory and run:

```powershell
dotnet test
```

Which should output something like:
```powershell
PS C:\code\FSM.Net\FSM.Net_i1\FrenchExDev.FiniteStateMachine.Net> dotnet test
Restauration terminée (0,9s)
  FrenchExDev.Net.FiniteStateMachine.Core a réussi (3,7s) → src\FrenchExDev.Net.FiniteStateMachine.Core\bin\Debug\net9.0\FrenchExDev.Net.FiniteStateMachine.Core.dll
  FrenchExDev.Net.FiniteStateMachine.Core.Tests a réussi (0,9s) → test\FrenchExDev.Net.FiniteStateMachine.Core.Tests\bin\Debug\net9.0\FrenchExDev.Net.FiniteStateMachine.Core.Tests.dll
  Test de FrenchExDev.Net.FiniteStateMachine.Core.Tests : a réussi (0,8 s)

Récapitulatif du test : total : 3; échec : 0; réussi : 3; ignoré : 0; durée : 0,4s
Générer a réussi dans 6,7s
```

---

Copyright (c) 2025 `FrenchExDev` `Stéphane ERARD` email@`stephane.erard@gmail.com`

Contact for licensing: `stephane.erard@gmail.com`.

This project is provided for `educational purposes only`.
