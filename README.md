# FrenchExDev.FiniteStateMachine.Net

A flexible, extensible Finite State Machine (FSM) library for .NET, written in C#.  
This library enables you to separate modeling and building of executable stateful workflows, with support for custom actions, conditions, and transition-specific behaviors.

---

## Features

- **Strongly-typed FSMs**: Use your own types for states, triggers, and context objects.
- **Builder Pattern**: Fluent API for defining states, transitions, and actions.
- **Transition Actions & Conditions**: Attach logic and guards to transitions.
- **State Entry Actions**: Execute logic when entering a state.
- **Unit-tested**: Includes comprehensive tests for typical FSM scenarios.

---

## Getting Started

### 1. Install

Clone the repository and add a reference to the `src/FrenchExDev.Net.FiniteStateMachine.Core` project in your solution.

### 2. Define Your Types

Define enums or classes for your states and triggers:

```csharp
enum DoorState { Closed, Open, Locked }
enum DoorEvent { Open, Close, Lock, Unlock }
```

### 3. Build a State Machine

Use the builder to define states and transitions:

```csharp
using FrenchExDev.Net.FiniteStateMachine.Core;

var builder = new FiniteStateMachineBuilder<Door, DoorState, DoorEvent>();

builder
    .CanTransition(fromState: DoorState.Closed, toState: DoorState.Open, on: DoorEvent.Open)
    .CanTransition(fromState: DoorState.Open, toState: DoorState.Closed, on: DoorEvent.Close)
    .CanTransition(fromState: DoorState.Closed, toState: DoorState.Locked, on: DoorEvent.Lock)
    .CanTransition(fromState: DoorState.Locked, toState: DoorState.Closed, on: DoorEvent.Unlock, body: (door, e, fsm) =>
    {
        door.Unlocked = true;
        // You can fire additional triggers or perform logic here
    });

var door = new Door();
var fsm = builder.Build(door, DoorState.Closed);
```

### 4. Fire triggers

```csharp
fsm.Fire(DoorEvent.Open);   // Door transitions to Open
fsm.Fire(DoorEvent.Close);  // Door transitions to Closed
fsm.Fire(DoorEvent.Lock);   // Door transitions to Locked
fsm.Fire(DoorEvent.Unlock); // Door transitions to Closed, sets door.Unlocked = true
```

_____


# Advanced: State Entry Actions

You can use the WhenFiniteStateMachineBuilder to define actions that run whenever a state is entered, regardless of the trigger:

```csharp
using FrenchExDev.Net.FiniteStateMachine.Core;

var builder = new WhenFiniteStateMachineBuilder<Device, DeviceState, DeviceEvent>();

builder
    .When(DeviceState.Connected, (device, trigger, fsm) =>
    {
        device.ResetAvailableOperations(new[] { DeviceOperation.Disconnect, DeviceOperation.Upload });
    })
    .CanTransition(on: DeviceEvent.Connect, fromState: DeviceState.Available, toState: DeviceState.Connected);

var device = new Device();
var fsm = builder.BuildWhen(device, DeviceState.Available);

fsm.Fire(DeviceEvent.Connect); // Device's available operations are reset on entering Connected
```

# Example: Device Lifecycle

```csharp
using FrenchExDev.Net.FiniteStateMachine.Core;

var builder = new FiniteStateMachineBuilder<Device, DeviceState, DeviceEvent>();

builder
    .CanTransition(on: DeviceEvent.Init, fromState: DeviceState.NotInitialized, toState: DeviceState.Initing, body: (device, e, fsm) =>
    {
        device.History(DeviceState.Initing, e, DateTime.UtcNow);
        fsm.Fire(DeviceEvent.Inited);
    })
    .CanTransition(on: DeviceEvent.Inited, fromState: DeviceState.Initing, toState: DeviceState.Available, body: (device, e, fsm) =>
    {
        device.History(DeviceState.Available, e, DateTime.UtcNow);
    })
    .CanTransition(on: DeviceEvent.Connect, fromState: DeviceState.Available, toState: DeviceState.Connected, body: (device, e, fsm) =>
    {
        device.History(DeviceState.Connected, e, DateTime.UtcNow);
    });

var device = new Device();
var fsm = builder.Build(device, DeviceState.NotInitialized);

fsm.Fire(DeviceEvent.Init);    // Device transitions to Initing, then to Available
fsm.Fire(DeviceEvent.Connect); // Device transitions to Connected
```


# Running Tests

Navigate to the test/FrenchExDev.Net.FiniteStateMachine.Core.Tests directory and run:

```powershell
dotnet test
```

---

Copyright (c) 2024 FrenchExDev Stéphane ERARD

This project is provided for educational purposes only.
