using Godot;
using System;

public partial class CameraController : Node3D
{
	[Export] public Node3D BodyNode { get; set; }
	[Export] public float CameraPanSpeed { get; set; }

	public override void _Ready()
	{
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent is InputEventMouseMotion motionEvent)
		{
			Vector2 delta = motionEvent.Relative;

			float newY = BodyNode.RotationDegrees.Y + (-delta.X * CameraPanSpeed);
			float newX = RotationDegrees.X + (delta.Y * CameraPanSpeed);

			newX = Mathf.Clamp(newX, -60, 50);

			BodyNode.RotationDegrees = new Vector3(0, newY, 0);

			RotationDegrees = new Vector3(newX, RotationDegrees.Y, 0);
		}

		if (inputEvent.IsActionPressed("ui_cancel"))
		{
			if (Input.MouseMode == Input.MouseModeEnum.Captured)
			{
				Input.MouseMode = Input.MouseModeEnum.Visible;
			}
			else
			{
                Input.MouseMode = Input.MouseModeEnum.Captured;
            }
		}
	}
}
