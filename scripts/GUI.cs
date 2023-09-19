using Godot;
using Godot.NativeInterop;
using System;

public partial class GUI : Control
{
    public override void _GuiInput(InputEvent @event)
    {
        //GD.Print("Event: ", @event);
    }

    public override void _Ready()
    {
        GetNode<Button>("MenuPanel/Choice/ActionsContainer/CenterContainer/StatsButton").GrabFocus();
    }
}
