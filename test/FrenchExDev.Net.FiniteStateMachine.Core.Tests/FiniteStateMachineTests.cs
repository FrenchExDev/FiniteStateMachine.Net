using Shouldly;

namespace FrenchExDev.Net.FiniteStateMachine.Core.Tests;

[TestClass]
public sealed class FiniteStateMachineTests
{
    internal enum DoorState { Closed, Open, Locked }
    internal enum DoorEvent { Open, Close, Lock, Unlock }
    internal class Door
    {
        public bool Unlocked { get; set; }
        public bool HasKey { get; set; }

        public Door()
        {
        }
    }

    [TestMethod]
    public void TestMethod1()
    {
        var doorFsmBuilder = new FiniteStateMachineBuilder<Door, DoorState, DoorEvent>();

        doorFsmBuilder
            .Transition(fromState: DoorState.Closed, toState: DoorState.Open, on: DoorEvent.Open)
            .Transition(fromState: DoorState.Open, toState: DoorState.Closed, on: DoorEvent.Close)
            .Transition(fromState: DoorState.Closed, toState: DoorState.Locked, on: DoorEvent.Lock)
            .Transition(fromState: DoorState.Locked, toState: DoorState.Closed, on: DoorEvent.Unlock, body: (door, e, fsm) =>
            {
                door.Unlocked = true;
                fsm.Fire(DoorEvent.Unlock).ShouldBeEquivalentTo(TransitionResult.InvalidTransition);
            })
            .Transition(on: DoorEvent.Open, fromState: DoorState.Locked, toState: DoorState.Open, condition: (door, e, fsm) => door.HasKey)
            ;

        var door1 = new Door();
        var door1fsm = doorFsmBuilder.Build(door1, DoorState.Closed);

        door1fsm.Fire(DoorEvent.Open).ShouldBeEquivalentTo(TransitionResult.Success);
        door1fsm.CurrentState.ShouldBeEquivalentTo(DoorState.Open);

        door1fsm.Fire(DoorEvent.Close).ShouldBeEquivalentTo(TransitionResult.Success);
        door1fsm.CurrentState.ShouldBeEquivalentTo(DoorState.Closed);

        door1fsm.Fire(DoorEvent.Lock).ShouldBeEquivalentTo(TransitionResult.Success);
        door1fsm.CurrentState.ShouldBeEquivalentTo(DoorState.Locked);

        door1fsm.Fire(DoorEvent.Unlock).ShouldBeEquivalentTo(TransitionResult.Success);
        door1fsm.CurrentState.ShouldBeEquivalentTo(DoorState.Closed);
        door1.Unlocked.ShouldBeTrue();
    }
}
