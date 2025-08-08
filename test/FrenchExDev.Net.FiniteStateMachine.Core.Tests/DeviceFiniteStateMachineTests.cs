using Shouldly;

namespace FrenchExDev.Net.FiniteStateMachine.Core.Tests;

/// <summary>
/// This test class demonstrates a finite state machine for an illustrative device lifecycle.
/// </summary>
[TestClass]
public sealed class DeviceFiniteStateMachineTests
{
    /// <summary>
    /// This enum defines the various states a device can be in during its lifecycle.
    /// </summary>
    [Flags]
    internal enum DeviceState
    {
        /// <summary>
        /// State indicating the device has not been initialized.
        /// </summary>
        NotInitialized,
        /// <summary>
        /// State indicating the device received an initialization request and is in the process of starting initializing.
        /// </summary>
        Init,
        /// <summary>
        /// State indicating the device is currently initializing.
        /// </summary
        Initing,
        /// <summary>
        /// State indicating the device has completed initialization
        /// </summary>
        Inited,
        /// <summary>
        /// State indicating the device is available for operations.
        /// </summary>
        Available,
        /// <summary>
        /// State indicating the device received a request to connect to a network or service, but has not yet started the connection process.
        /// </summary>
        Connect,
        /// <summary>
        /// State indicating the device is in the process of connecting to a network or service.
        /// </summary>
        Connecting,
        /// <summary>
        /// State indicating the device has successfully connected to a network or service.
        /// </summary>
        Connected,
        /// <summary>
        /// State indicating the device received a request to disconnect from a network or service.
        /// </summary>
        Disconnect,
        /// <summary>
        /// State indicating the device is in the process of disconnecting from a network or service.
        /// </summary>
        Disconnecting,
        /// <summary>
        ///  State indicating the device has successfully disconnected from a network or service.
        /// </summary>
        Disconnected,
        /// <summary>
        /// State indicating the device is currently online
        /// </summary>
        Online,
        /// <summary>
        /// State indicating the device is in the process of switching to an online state.
        /// </summary>
        SwitchingOnline,
        /// <summary>
        /// State indicating the device has successfully switched to an offline state.
        /// </summary>
        Offline,
        /// <summary>
        /// State indicating the device is in the process of switching to an offline state.
        /// </summary>
        SwitchingOffline,
        UploadingConfig,
        UploadedConfig,
        DeployingConfig,
        DeployedConfig,

    }

    /// <summary>
    /// Represents a record of a device's state at a specific point in time.
    /// </summary>
    /// <param name="State">The current state of the device.</param>
    /// <param name="Device">The device associated with this state record.</param>
    /// <param name="TimeStamp">The date and time when the state was recorded.</param>
    internal record DeviceStateRecord(DeviceState State, Device Device, DateTime TimeStamp);

    /// <summary>
    /// Represents a record of a device event, including the associated device, timestamp, and optional device state.
    /// </summary>
    /// <param name="Event">The event that occurred on the device.</param>
    /// <param name="Device">The device associated with the event.</param>
    /// <param name="TimeStamp">The date and time when the event occurred.</param>
    /// <param name="State">The optional state of the device at the time of the event. Can be <see langword="null"/> if no state information
    /// is available.</param>
    internal record DeviceEventRecord(DeviceEvent Event, Device Device, DateTime TimeStamp, DeviceStateRecord? State);

    /// <summary>
    /// Represents the various events or states that a device can transition through during its lifecycle.
    /// </summary>
    /// <remarks>This enumeration defines a set of events and states that describe the operational lifecycle
    /// of a device,  including initialization, connection, online/offline transitions, configuration updates, and
    /// availability. These values can be used to track or respond to changes in the device's state.</remarks>
    internal enum DeviceEvent
    {
        Init,
        Initing,
        Inited,
        Connect,
        Connecting,
        Connected,
        Disconnect,
        Disconnecting,
        Disconnected,
        Online,
        SwitchingOnline,
        SwitchedOnline,
        Offline,
        SwitchingOffline,
        SwitchedOffline,
        UploadingConfig,
        UploadedConfig,
        DeployingConfig,
        DeployedConfig,
        Available,
    }

    /// <summary>
    /// Represents a device that maintains a history of its states and events.
    /// </summary>
    /// <remarks>The <see cref="Device"/> class provides functionality to record and retrieve the history of 
    /// state changes and events associated with the device. State and event records are stored  internally and can be
    /// accessed through the <see cref="StateHistory"/> and <see cref="EventHistory"/>  properties,
    /// respectively.</remarks>
    internal class Device
    {
        /// <summary>
        /// Represents the history of device state changes.
        /// </summary>
        /// <remarks>This collection stores a chronological record of device state changes,  allowing for
        /// tracking and analysis of state transitions over time.</remarks>
        private readonly List<DeviceStateRecord> _stateHistory = new();

        /// <summary>
        /// Represents the history of device events.
        /// </summary>
        /// <remarks>This collection stores a record of all device events that have occurred.  It is used
        /// internally to track and manage event history for the device.</remarks>
        private readonly List<DeviceEventRecord> _eventHistory = new();

        /// <summary>
        /// Gets the historical sequence of device state records.
        /// </summary>
        public IEnumerable<DeviceStateRecord> StateHistory => _stateHistory;

        /// <summary>
        /// Gets the collection of historical device events.
        /// </summary>
        public IEnumerable<DeviceEventRecord> EventHistory => _eventHistory;


        /// <summary>
        /// Adds a record of the device's actual state in its history.
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public Device History(DeviceStateRecord record)
        {
            _stateHistory.Add(record);
            return this;
        }

        /// <summary>
        /// Adds the specified event record to the device's event history.
        /// </summary>
        /// <param name="record">The event record to add to the device's history. Cannot be null.</param>
        /// <returns>The current <see cref="Device"/> instance, allowing for method chaining.</returns>
        public Device History(DeviceEventRecord record)
        {
            _eventHistory.Add(record);
            return this;
        }

        /// <summary>
        /// Adds a record of the device's state and event to its history.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="event"></param>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public Device History(DeviceState state, DeviceEvent @event, DateTime timeStamp)
        {
            var stateRecord = new DeviceStateRecord(state, this, timeStamp);
            var eventRecord = new DeviceEventRecord(@event, this, timeStamp, stateRecord);
            History(stateRecord)
                .History(eventRecord);

            return this;
        }
    }

    /// <summary>
    /// This test method demonstrates the lifecycle of a device using a finite state machine.
    /// </summary>
    [TestMethod]
    public void TestMethod1()
    {
        // Instantiate the finite state machine builder for the Device class, using DeviceState and DeviceEvent as the state and event types.
        var fsmBuilder = new FiniteStateMachineBuilder<Device, DeviceState, DeviceEvent>();

        // Define the transitions for the finite state machine, specifying the events, states, and actions to be taken during each transition.
        fsmBuilder
            // Define the initial state of the device as NotInitialized.
            .CanTransition(on: DeviceEvent.Init, fromState: DeviceState.NotInitialized, toState: DeviceState.Initing, body: (device, e, fsm) =>
            {
                // Record the current state and event in the device's history.
                device.History(state: fsm.CurrentState, @event: e, timeStamp: DateTime.UtcNow);
                // Fire the Initing event to transition to the Initing state.
                fsm.Fire(DeviceEvent.Initing).ShouldBeEquivalentTo(TransitionResult.Success);
            })
            // Define the transition from Initing to Inited state.
            .CanTransition(on: DeviceEvent.Initing, fromState: DeviceState.Initing, toState: DeviceState.Inited, body: (device, e, fsm) =>
            {
                // Record the current state and event in the device's history.
                device.History(state: fsm.CurrentState, @event: e, timeStamp: DateTime.UtcNow);
                // Fire the Inited event to transition to the Inited state.
                fsm.Fire(DeviceEvent.Inited).ShouldBeEquivalentTo(TransitionResult.Success);
            })
            // Define the transition from Inited to Available state.
            .CanTransition(on: DeviceEvent.Inited, fromState: DeviceState.Inited, toState: DeviceState.Available, body: (device, e, fsm) =>
            {
                // Record the current state and event in the device's history.
                device.History(state: fsm.CurrentState, @event: e, timeStamp: DateTime.UtcNow);
            })
            // Define the transition from Available to Connecting state.
            .CanTransition(on: DeviceEvent.Connect, fromState: DeviceState.Available, toState: DeviceState.Connecting, body: (device, e, fsm) =>
            {
                // Record the current state and event in the device's history.
                device.History(state: fsm.CurrentState, @event: e, timeStamp: DateTime.UtcNow);
                // Fire the Connecting event to transition to the Connecting state.
                fsm.Fire(DeviceEvent.Connecting).ShouldBeEquivalentTo(TransitionResult.Success);
            })
            // Define the transition from Connecting to Connected state.
            .CanTransition(on: DeviceEvent.Connecting, fromState: DeviceState.Connecting, toState: DeviceState.Connected, body: (device, e, fsm) =>
            {
                // Record the current state and event in the device's history.
                device.History(state: fsm.CurrentState, @event: e, timeStamp: DateTime.UtcNow);
                // Fire the Connected event to transition to the Connected state.
                fsm.Fire(DeviceEvent.Online).ShouldBeEquivalentTo(TransitionResult.Success);
            })
            // Define the transition from Connected to Online state.
            .CanTransition(on: DeviceEvent.Online, fromState: DeviceState.Connected, toState: DeviceState.Online, body: (device, e, fsm) =>
            {
                // Record the current state and event in the device's history.
                device.History(state: fsm.CurrentState, @event: e, timeStamp: DateTime.UtcNow);
            })
            // Define the transition from Online to Disconnecting state.
            .CanTransition(on: DeviceEvent.Disconnect, fromState: DeviceState.Online, toState: DeviceState.Available, body: (device, e, fsm) =>
            {
                // Record the current state and event in the device's history.
                device.History(state: fsm.CurrentState, @event: e, timeStamp: DateTime.UtcNow);
            })
            // Define the transition from Available to Disconnecting state.
            .CanTransition(on: DeviceEvent.Disconnected, fromState: DeviceState.Disconnecting, toState: DeviceState.Disconnected, body: (device, e, fsm) =>
            {
                // Record the current state and event in the device's history.
                device.History(state: fsm.CurrentState, @event: e, timeStamp: DateTime.UtcNow);
                // Fire the Available event to transition to the Available state.
                fsm.Fire(DeviceEvent.Available).ShouldBeEquivalentTo(TransitionResult.Success);
            })
            // Define the transition from Disconnected to Available state.
            .CanTransition(on: DeviceEvent.Available, fromState: DeviceState.Disconnected, toState: DeviceState.Available, body: (device, e, fsm) =>
            {
                // Record the current state and event in the device's history.
                device.History(state: fsm.CurrentState, @event: e, timeStamp: DateTime.UtcNow);
            });


        // Instantiate a new Device object to represent the device being managed by the finite state machine.
        var device = new Device();

        // Build the finite state machine for the Device class, starting from the NotInitialized state.
        var deviceFsm = fsmBuilder.Build(device, DeviceState.NotInitialized);

        //Fire the Init event to transition the device from NotInitialized to Initing state.
        deviceFsm.Fire(DeviceEvent.Init).ShouldBeEquivalentTo(TransitionResult.Success);
        // Verify that the current state of the device is Available.
        deviceFsm.CurrentState.ShouldBeEquivalentTo(DeviceState.Available);

        // Fire the Connect event to transition the device from Available to Online state.
        deviceFsm.Fire(DeviceEvent.Connect).ShouldBeEquivalentTo(TransitionResult.Success);
        // Verify that the current state of the device is Online.
        deviceFsm.CurrentState.ShouldBeEquivalentTo(DeviceState.Online);

        // Verify that the device's state history is not empty and contains the expected states.
        device.StateHistory.ShouldNotBeEmpty();

        var i = 0;
        device.StateHistory.ElementAt(i++).State.ShouldBeEquivalentTo(DeviceState.Initing);
        device.StateHistory.ElementAt(i++).State.ShouldBeEquivalentTo(DeviceState.Inited);
        device.StateHistory.ElementAt(i++).State.ShouldBeEquivalentTo(DeviceState.Available);
        device.StateHistory.ElementAt(i++).State.ShouldBeEquivalentTo(DeviceState.Connecting);
        device.StateHistory.ElementAt(i++).State.ShouldBeEquivalentTo(DeviceState.Connected);
        device.StateHistory.ElementAt(i++).State.ShouldBeEquivalentTo(DeviceState.Online);

        // Verify that the device's event history is not empty.
        device.EventHistory.ShouldNotBeEmpty();
    }
}