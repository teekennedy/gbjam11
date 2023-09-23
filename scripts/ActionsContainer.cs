using Godot;
using System;

public partial class ActionsContainer : HFlowContainer
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("ActionsContainer ready");
        // Focus first child node

        bool FirstButtonGrabFocus(Node node)
        {
            if (node is Button)
            {
                Button button = node as Button;
                button.GrabFocus();
                return false;
            }
            return true;
        }
        WalkChildren(this, FirstButtonGrabFocus);
    }

    public void WalkChildren(Node node, Func<Node, bool> walkFunc)
    {

        if (!walkFunc(node))
        {
            return;
        }
        bool keepWalking = true;
        bool wrappedWalkFunc(Node node)
        {
            if (!walkFunc(node))
            {
                keepWalking = false;
            }
            return keepWalking;
        }
        foreach (var child in node.GetChildren())
        {
            WalkChildren(child, wrappedWalkFunc);
            if (!keepWalking)
            {
                return;
            }
        }

    }
}
