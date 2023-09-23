using Godot;
using System;

public partial class FocusedControl : Control
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("FocusedControl ready");
        // Focus first child node

        static bool FirstButtonGrabFocus(Node node)
        {
            if (node is Button)
            {
                Button button = node as Button;
                GD.Print("Found button node!");
                button.GrabFocus();
                return false;
            }
            else
            {
                GD.Print(node.GetPath());
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