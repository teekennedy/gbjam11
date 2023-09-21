using Godot;
using System;

public partial class Message : RichTextLabel
{
	// Signals that the message is done displaying.
	[Signal]
	public delegate void MessageFinishedEventHandler();

	// The line number of the first line of the text box.
	private int _CurrentLine = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//start sound for typing
		GetChild<AudioStreamPlayer>(1).Play();

		var continueButton = GetNode<Button>("%ContinueButton");
		var textAnimation = GetChild<AnimationPlayer>(0);
		continueButton.GrabFocus();
		void continueMessage()
		{
            //stop sound
            GetChild<AudioStreamPlayer>(1).Stop();

            // If textAnimation is done
            if (!textAnimation.IsPlaying())
			{
				// Is there more text to show?
				if (GetLineCount() > GetVisibleLineCount())
				{
					GD.Print("TODO: handle messages that are too big to fit in one text box.");
				}
				EmitSignal(SignalName.MessageFinished);
			}
			else
			{
				// Stop the animation while keeping the visual state
				textAnimation.Stop(true);
				// Make the entire message visible
				VisibleRatio = 1.0f;
				// Make continue button visible
				continueButton.Visible = true;
            }
		}
		continueButton.Pressed += continueMessage;
		textAnimation.AnimationFinished += (_) => continueButton.Visible = true;
        textAnimation.AnimationFinished += (_) => GetChild<AudioStreamPlayer>(1).Stop();

    }
}
