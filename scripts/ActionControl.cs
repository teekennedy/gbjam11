using Godot;
using System;

public partial class ActionControl : Control
{
    [Signal]
    public delegate void ActionSelectedEventHandler(string action);

    public void OnActionButtonPressed(string action)
    {
        GD.Print("Action button pressed: ", action);
        EmitSignal(SignalName.ActionSelected, action);
    }
}
