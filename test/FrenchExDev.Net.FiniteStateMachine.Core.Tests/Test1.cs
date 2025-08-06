using Shouldly;

namespace FrenchExDev.Net.FiniteStateMachine.Core.Tests;

[TestClass]
public sealed class Test1
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

        doorFsmBuilder.Transition(fromState: DoorState.Closed, toState: DoorState.Open, on: DoorEvent.Open);
        doorFsmBuilder.Transition(fromState: DoorState.Open, toState: DoorState.Closed, on: DoorEvent.Close);
        doorFsmBuilder.Transition(fromState: DoorState.Closed, toState: DoorState.Locked, on: DoorEvent.Lock);
        doorFsmBuilder.Transition(fromState: DoorState.Locked, toState: DoorState.Closed, on: DoorEvent.Unlock, body: (door) =>
        {
            door.Unlocked = true;
        });
        doorFsmBuilder.Transition(DoorState.Locked, DoorState.Open, DoorEvent.Open, condition: (door) => door.HasKey);

        var door1 = new Door();
        var door1fsm = doorFsmBuilder.Build(door1, DoorState.Closed);

        door1fsm.Fire(DoorEvent.Open);
        door1fsm.CurrentState.ShouldBeEquivalentTo(DoorState.Open);

        door1fsm.Fire(DoorEvent.Close);
        door1fsm.CurrentState.ShouldBeEquivalentTo(DoorState.Closed);

        door1fsm.Fire(DoorEvent.Lock);
        door1fsm.CurrentState.ShouldBeEquivalentTo(DoorState.Locked);

        door1fsm.Fire(DoorEvent.Unlock);
        door1.Unlocked.ShouldBeTrue();
    }
}
