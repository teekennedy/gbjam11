using Godot;
using System;

public partial class ShipAnimationTree : AnimationTree
{
    // Reference to this AnimationTree's state machine.
    private AnimationNodeStateMachinePlayback stateMachine;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        stateMachine = (AnimationNodeStateMachinePlayback)Get("parameters/playback");

        // Connect the ActionStarted signal to play specific animations
        var tracker = GetNode<Tracker>("/root/Tracker");
        tracker.ActionStarted += OnTrackerActionStarted;
        AnimationFinished += (name) => GD.Print("Animation finished", name);
    }

    private void OnTrackerActionStarted(int action)
    {
        var actionType = (Tracker.ActionType)action;
        switch (actionType)
        {
            case Tracker.ActionType.Raided:
                stateMachine.Travel("enemy_raid");
                break;
            default:
                GD.Print("No animation for ", actionType.ToString());
                break;
        }
    }
}
