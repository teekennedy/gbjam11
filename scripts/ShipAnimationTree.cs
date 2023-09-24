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
        AnimationFinished += (name) => GD.Print("Animation finished: ", name);
        AnimationStarted += (name) => GD.Print("Animation  started: ", name);
    }

    private void OnTrackerActionStarted(int action)
    {
        var actionType = (Tracker.ActionType)action;

        GD.Print("ShipAnimationTree got action: ", actionType.ToString());
        switch (actionType)
        {
            case Tracker.ActionType.Idle:
                stateMachine.Travel("ship_idle");
                break;
            case Tracker.ActionType.Raided:
                stateMachine.Travel("enemy_raid");
                break;
            case Tracker.ActionType.Scan:
                stateMachine.Travel("ship_scan");
                break;
            case Tracker.ActionType.Travel:
                stateMachine.Travel("ship_travel");
                break;
            default:
                break;
        }
    }
}
