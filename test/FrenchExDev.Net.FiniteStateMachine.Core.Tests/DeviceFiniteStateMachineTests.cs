using Shouldly;

namespace FrenchExDev.Net.FiniteStateMachine.Core.Tests;

[TestClass]
public sealed class DeviceFiniteStateMachineTests
{
    [Flags]
    internal enum DeviceState
    {
        NotInitialized,
        Init,
        Initing,
        Inited,
        Connecting,
        Connected,
        Disconnecting,
        Disconnected,
        Online,
        SwitchingOnline,
        Offline,
        SwitchingOffline,
        UploadingConfig,
        UploadedConfig,
        DeployingConfig,
        DeployedConfig,
        Available
    }

    internal record DeviceStateRecord(DeviceState State, Device Device, DateTime TimeStamp);
    internal record DeviceEventRecord(DeviceEvent Event, Device Device, DateTime TimeStamp, DeviceStateRecord? State);

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

    internal class Device
    {
        private readonly List<DeviceStateRecord> _stateHistory = new();
        private readonly List<DeviceEventRecord> _eventHistory = new();
        public IEnumerable<DeviceStateRecord> StateHistory => _stateHistory;
        public IEnumerable<DeviceEventRecord> EventHistory => _eventHistory;

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
    }


    [TestMethod]
    public void TestMethod1()
    {
        var fsmBuilder = new FiniteStateMachineBuilder<Device, DeviceState, DeviceEvent>();

        fsmBuilder
            .CanTransition(on: DeviceEvent.Init, fromState: DeviceState.NotInitialized, toState: DeviceState.Initing, body: (device, e, fsm) =>
            {
                device.History(state: fsm.CurrentState, @event: e, timeStamp: DateTime.UtcNow);
                fsm.Fire(DeviceEvent.Initing).ShouldBeEquivalentTo(TransitionResult.Success);
            })
            .CanTransition(on: DeviceEvent.Initing, fromState: DeviceState.Initing, toState: DeviceState.Inited, body: (device, e, fsm) =>
            {
                device.History(state: fsm.CurrentState, @event: e, timeStamp: DateTime.UtcNow);
                fsm.Fire(DeviceEvent.Inited).ShouldBeEquivalentTo(TransitionResult.Success);
            })
            .CanTransition(on: DeviceEvent.Inited, fromState: DeviceState.Inited, toState: DeviceState.Available, body: (device, e, fsm) =>
            {
                device.History(state: fsm.CurrentState, @event: e, timeStamp: DateTime.UtcNow);
            })
            .CanTransition(on: DeviceEvent.Connect, fromState: DeviceState.Available, toState: DeviceState.Connecting, body: (device, e, fsm) =>
            {
                device.History(state: fsm.CurrentState, @event: e, timeStamp: DateTime.UtcNow);
                fsm.Fire(DeviceEvent.Connecting).ShouldBeEquivalentTo(TransitionResult.Success);
            })
            .CanTransition(on: DeviceEvent.Connecting, fromState: DeviceState.Connecting, toState: DeviceState.Connected, body: (device, e, fsm) =>
            {
                device.History(state: fsm.CurrentState, @event: e, timeStamp: DateTime.UtcNow);
                fsm.Fire(DeviceEvent.Online).ShouldBeEquivalentTo(TransitionResult.Success);
            })
            .CanTransition(on: DeviceEvent.Online, fromState: DeviceState.Connected, toState: DeviceState.Online, body: (device, e, fsm) =>
            {
                device.History(state: fsm.CurrentState, @event: e, timeStamp: DateTime.UtcNow);
            })
            .CanTransition(on: DeviceEvent.Disconnect, fromState: DeviceState.Online, toState: DeviceState.Available, body: (device, e, fsm) =>
            {
                device.History(state: fsm.CurrentState, @event: e, timeStamp: DateTime.UtcNow);
            })
            .CanTransition(on: DeviceEvent.Disconnected, fromState: DeviceState.Disconnecting, toState: DeviceState.Disconnected, body: (device, e, fsm) =>
            {
                device.History(state: fsm.CurrentState, @event: e, timeStamp: DateTime.UtcNow);
                fsm.Fire(DeviceEvent.Available).ShouldBeEquivalentTo(TransitionResult.Success);
            })
            .CanTransition(on: DeviceEvent.Available, fromState: DeviceState.Disconnected, toState: DeviceState.Available, body: (device, e, fsm) =>
            {
                device.History(state: fsm.CurrentState, @event: e, timeStamp: DateTime.UtcNow);
            });

        var device = new Device();
        var deviceFsm = fsmBuilder.Build(device, DeviceState.NotInitialized);

        deviceFsm.Fire(DeviceEvent.Init).ShouldBeEquivalentTo(TransitionResult.Success);
        deviceFsm.CurrentState.ShouldBeEquivalentTo(DeviceState.Available);

        deviceFsm.Fire(DeviceEvent.Connect).ShouldBeEquivalentTo(TransitionResult.Success);
        deviceFsm.CurrentState.ShouldBeEquivalentTo(DeviceState.Online);

        device.StateHistory.ShouldNotBeEmpty();

        var i = 0;
        device.StateHistory.ElementAt(i++).State.ShouldBeEquivalentTo(DeviceState.Initing);
        device.StateHistory.ElementAt(i++).State.ShouldBeEquivalentTo(DeviceState.Inited);
        device.StateHistory.ElementAt(i++).State.ShouldBeEquivalentTo(DeviceState.Available);
        device.StateHistory.ElementAt(i++).State.ShouldBeEquivalentTo(DeviceState.Connecting);
        device.StateHistory.ElementAt(i++).State.ShouldBeEquivalentTo(DeviceState.Connected);
        device.StateHistory.ElementAt(i++).State.ShouldBeEquivalentTo(DeviceState.Online);

        device.EventHistory.ShouldNotBeEmpty();
    }
}