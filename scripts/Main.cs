using Godot;
using System;
using System.CodeDom.Compiler;

public partial class Main : AspectRatioContainer
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Set the window to 4x normal GameBoy screen size
        GetWindow().Size = new Vector2I(160 * 4, 144 * 4);

        // Whatever current scene is running, move it in the scene tree to be a child of the GBScreen SubViewport node.
        var parentNode = GetParent();
        var lastNodeIndex = parentNode.GetChildCount() - 1;
        // Get the node for the currently running scene
        var sceneNode = parentNode.GetChild<CanvasItem>(lastNodeIndex);
        // Is this node the currently running scene?
        if (sceneNode == this)
        {
            GD.Print("Couldn't find another running scene to place under GBScreen");
            return;
        }
        // Get the Game node to place it under
        var gameNode = GetNode<Node>("%Game");
        // If the loaded scene is a duplicate Game scene, remove it
        if (sceneNode.Name == "Game")
        {
            GD.Print("Removing duplicate Game scene");
            sceneNode.QueueFree();
            return;
        }
        // Move scene from the root node to the Game node.
        // These calls must be CallDeferred because all nodes are currently processing their _Ready() function.
        parentNode.CallDeferred("remove_child", sceneNode);
        // Request that the _Ready func be called again once the node is added to the node tree
        sceneNode.CallDeferred("request_ready");
        // Add scene as child of Game
        gameNode.CallDeferred("add_child", sceneNode);
        var tracker = GetNode<Tracker>("/root/Tracker");
        tracker._ActionNode = sceneNode;
    }
}
