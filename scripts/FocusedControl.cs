using Godot;
using System;

public partial class FocusedControl : Control
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //GD.Print("FocusedControl _Ready");
        // Function that calls GrabFocus() on the first button node
        static bool FirstButtonGrabFocus(Node node)
        {
            // GD.Print(node.GetPath());
            // Have we found a button?
            if (node is Button)
            {
                // Call GrabFocus on button
                Button button = node as Button;
                button.GrabFocus();
                // Stop checking nodes
                return false;
            }
            // Continue searching for button
            return true;
        }
        // Run the above function against all nodes that are chilren of this node.
        WalkChildren(this, FirstButtonGrabFocus);
    }

    // Calls walkFunc once for every direct and indirect child of node, depth-first.
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