using Shouldly;

namespace FrenchExDev.Net.FiniteStateMachine.Core.Tests;

[TestClass]
public class DeviceWhenFiniteStateMachineTests
{
    internal enum DeviceState
    {
        NotAvailable,
        Available,
        Disconnect,
        Disconnecting,
        Disconnected,
        Connect,
        Connecting,
        Connected,
        SwitchOnline,
        SwitchingOnline,
        SwitchedOnline,
        SwitchOffline,
        SwitchingOffline,
        SwitchedOffline,
    }

    internal enum DeviceConnectionStatus
    {
        Online,
        Offline
    }

    internal record DeviceStateRecord(DeviceState State, Device Device, DateTime TimeStamp);
    internal record DeviceEventRecord(DeviceEvent Event, Device Device, DateTime TimeStamp, DeviceStateRecord? State);
    internal record DeviceAvailableOperationsRecord(IEnumerable<DeviceOperation> Operations, Device Device, DateTime TimeStamp, DeviceStateRecord? State);

    internal enum DeviceEvent
    {
        Init,
        Available,
        Connected,
        Offline,
        Online,
        Connecting,
        Connect,
        SwitchOnline,
        SwitchOffline
    }

    internal enum DeviceOperation
    {
        Connect,
        Disconnect,
        Download,
        Upload,
        SwitchOnline,
        SwitchOffline
    }

    internal class Device
    {
        private readonly List<DeviceStateRecord> _stateHistory = new();
        private readonly List<DeviceEventRecord> _eventHistory = new();
        private readonly List<DeviceAvailableOperationsRecord> _availableOperationsHistory = new();
        private List<DeviceOperation> _canDo = new();
        public IReadOnlyList<DeviceStateRecord> StateHistory => _stateHistory;
        public IReadOnlyList<DeviceEventRecord> EventHistory => _eventHistory;
        public IReadOnlyList<DeviceOperation> AvailableOperations => _canDo;
        public DeviceConnectionStatus ConnectionStatus { get; set; } = DeviceConnectionStatus.Offline;

        public Device History(DeviceAvailableOperationsRecord record)
        {
            _availableOperationsHistory.Add(record);

            return this;
        }

        public Device History(DeviceStateRecord record)
        {
            _stateHistory.Add(record);

            return this;
        }

        public Device History(DeviceEventRecord record)
        {
            _eventHistory.Add(record);

            return this;
        }

        public Device History(DeviceState state, DeviceEvent @event, DateTime timeStamp)
        {
            var stateRecord = new DeviceStateRecord(state, this, timeStamp);
            var eventRecord = new DeviceEventRecord(@event, this, timeStamp, stateRecord);
            History(stateRecord)
                .History(eventRecord);

            return this;
        }

        public Device ResetAvailableOperations(IEnumerable<DeviceOperation> operations)
        {
            _canDo = operations.ToList();
            History(new DeviceAvailableOperationsRecord(_canDo, this, DateTime.UtcNow, null));

            return this;
        }
    }

    [TestMethod]
    public void TestMethod1()
    {
        var fsmBuilder = new WhenFiniteStateMachineBuilder<Device, DeviceState, DeviceEvent>();

        List<DeviceOperation> whenConnectedAndIsOfflineOperations = [DeviceOperation.Disconnect, DeviceOperation.SwitchOnline, DeviceOperation.Upload, DeviceOperation.Download];
        List<DeviceOperation> whenConnectedAndIsOnlineOperations = [DeviceOperation.Disconnect, DeviceOperation.SwitchOffline, DeviceOperation.Upload, DeviceOperation.Download];

        fsmBuilder
            .When(DeviceState.Disconnected, (device, e, fsm) =>
            {
                device.ResetAvailableOperations([DeviceOperation.Connect]);
            })
            .When(DeviceState.Connected, (device, e, fsm) =>
            {
                switch (device.ConnectionStatus)
                {
                    case DeviceConnectionStatus.Online:
                        device.ResetAvailableOperations(whenConnectedAndIsOnlineOperations);
                        break;
                    case DeviceConnectionStatus.Offline:
                        device.ResetAvailableOperations(whenConnectedAndIsOfflineOperations);
                        break;
                }
            });

        fsmBuilder
            .CanTransition(on: DeviceEvent.Init, fromState: DeviceState.NotAvailable, toState: DeviceState.Available, body: (device, e, fsm) =>
            {
                device.History(state: fsm.CurrentState, @event: e, timeStamp: DateTime.UtcNow);
            })
            .CanTransition(on: DeviceEvent.Connect, fromState: DeviceState.Available, toState: DeviceState.Connected, body: (device, e, fsm) =>
            {
                device.History(state: fsm.CurrentState, @event: e, timeStamp: DateTime.UtcNow);
            })
            .CanTransition(on: DeviceEvent.SwitchOnline, fromState: DeviceState.Connected, toState: DeviceState.SwitchedOnline, body: (device, e, fsm) =>
            {
                device.ConnectionStatus = DeviceConnectionStatus.Online;
                device.History(state: fsm.CurrentState, @event: e, timeStamp: DateTime.UtcNow);
                fsm.Fire(DeviceEvent.Connected).ShouldBeEquivalentTo(TransitionResult.Success);
            })
            .CanTransition(on: DeviceEvent.Connected, fromState: DeviceState.SwitchedOnline, toState: DeviceState.Connected, body: (device, e, fsm) =>
            {
                device.History(state: fsm.CurrentState, @event: e, timeStamp: DateTime.UtcNow);
            })
            .CanTransition(on: DeviceEvent.Connected, fromState: DeviceState.SwitchedOnline, toState: DeviceState.Available, body: (device, e, fsm) =>
            {
                device.History(state: fsm.CurrentState, @event: e, timeStamp: DateTime.UtcNow);
            })
            .CanTransition(on: DeviceEvent.SwitchOffline, fromState: DeviceState.Connected, toState: DeviceState.SwitchedOffline, body: (device, e, fsm) =>
            {
                device.ConnectionStatus = DeviceConnectionStatus.Offline;
                device.History(state: fsm.CurrentState, @event: e, timeStamp: DateTime.UtcNow);
                fsm.Fire(DeviceEvent.Connect).ShouldBeEquivalentTo(TransitionResult.Success);
            })
            .CanTransition(on:DeviceEvent.Connect, fromState: DeviceState.SwitchedOffline, toState: DeviceState.Connected, body: (device, e, fsm) =>
            {
                device.History(state: fsm.CurrentState, @event: e, timeStamp: DateTime.UtcNow);
            })
            .CanTransition(on: DeviceEvent.Connected, fromState: DeviceState.SwitchedOffline, toState: DeviceState.Available, body: (device, e, fsm) =>
            {
                device.History(state: fsm.CurrentState, @event: e, timeStamp: DateTime.UtcNow);
            })
           ;

        var device = new Device();
        IWhenFiniteStateMachine<Device, DeviceState, DeviceEvent> deviceFsm = fsmBuilder.BuildWhen(device, DeviceState.NotAvailable);

        deviceFsm.ShouldNotBeNull();

        device.ConnectionStatus.ShouldBeEquivalentTo(DeviceConnectionStatus.Offline);

        deviceFsm.Fire(DeviceEvent.Init).ShouldBeEquivalentTo(TransitionResult.Success);
        deviceFsm.CurrentState.ShouldBeEquivalentTo(DeviceState.Available);

        deviceFsm.Fire(DeviceEvent.Connect).ShouldBeEquivalentTo(TransitionResult.Success);
        deviceFsm.CurrentState.ShouldBeEquivalentTo(DeviceState.Connected);

        var availableOperations = device.AvailableOperations.ToList();

        foreach (var operation in whenConnectedAndIsOfflineOperations)
        {
            availableOperations.ShouldContain(operation);
        }

        deviceFsm.Fire(DeviceEvent.SwitchOnline).ShouldBeEquivalentTo(TransitionResult.Success);
        deviceFsm.CurrentState.ShouldBeEquivalentTo(DeviceState.Connected);
        device.ConnectionStatus.ShouldBeEquivalentTo(DeviceConnectionStatus.Online);

        deviceFsm.Fire(DeviceEvent.SwitchOffline).ShouldBeEquivalentTo(TransitionResult.Success);
        deviceFsm.CurrentState.ShouldBeEquivalentTo(DeviceState.Connected);
        device.ConnectionStatus.ShouldBeEquivalentTo(DeviceConnectionStatus.Offline);

        deviceFsm.Fire(DeviceEvent.SwitchOnline).ShouldBeEquivalentTo(TransitionResult.Success);
        deviceFsm.CurrentState.ShouldBeEquivalentTo(DeviceState.Connected);
        device.ConnectionStatus.ShouldBeEquivalentTo(DeviceConnectionStatus.Online);

        deviceFsm.Fire(DeviceEvent.SwitchOffline).ShouldBeEquivalentTo(TransitionResult.Success);
        deviceFsm.CurrentState.ShouldBeEquivalentTo(DeviceState.Connected);
        device.ConnectionStatus.ShouldBeEquivalentTo(DeviceConnectionStatus.Offline);

        device.EventHistory.ShouldNotBeEmpty();
    }
}
