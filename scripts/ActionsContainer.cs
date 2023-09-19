using Godot;
using System;

public partial class ActionsContainer : HFlowContainer
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Focus first child node
        var child = GetChildOrNull<Button>(0);
        child?.GrabFocus();
    }
}
