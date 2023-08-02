using Godot;
using System;

public partial class CameraController : Node
{
	[Export] public Node3D CameraContainerNode { get; set; }
    [Export] public Node3D CameraPivotNode { get; set; }
    [Export] public Node3D BodyNode { get; set; }

	[Export] public float CameraPanDesiredVerticalSpeed { get; set; } = 3;
	[Export] public float CameraPanDesiredHorizontalSpeed { get; set; } = 0.05f;
	[Export] public float CameraPanBlendSpeed { get; set; } = 0.5f;

    [Export] public float CameraHorizontalMoveSpeed { get; set; }
    [Export] public float CameraVerticalMoveSpeed { get; set; }
    [Export] public float CameraMovementDampening { get; set; }
    [Export] public float CameraChangeSpeed { get; set; }
    [Export] public float MaxAllowedDistance { get; set; }

	private Vector3 targetRotation = Vector3.Zero;
	private Vector3 currentVelocity = Vector3.Zero;

    public override void _Ready()
	{
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent is InputEventMouseMotion motionEvent)
		{
			Vector2 delta = motionEvent.Relative;

			targetRotation.Y = Mathf.Wrap((-1 * delta.X * CameraPanDesiredHorizontalSpeed) + targetRotation.Y, 0, 360);
			targetRotation.X = (-1 * delta.Y * CameraPanDesiredVerticalSpeed) + CameraPivotNode.RotationDegrees.X;

			targetRotation.X = Mathf.Clamp(targetRotation.X, -60, 50);
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

	public override void _PhysicsProcess(double delta)
	{
		Vector3 currentDir = (BodyNode.GlobalPosition - CameraContainerNode.GlobalPosition).Normalized();
		currentDir.X *= CameraHorizontalMoveSpeed;
        currentDir.Y *= CameraVerticalMoveSpeed;
        currentDir.Z *= CameraHorizontalMoveSpeed;

		currentVelocity += (currentDir - currentVelocity) * (float)delta * CameraChangeSpeed;
		currentVelocity *= CameraMovementDampening;

		float currentDistanceToBody = CameraContainerNode.GlobalPosition.DistanceTo(BodyNode.GlobalPosition);

		if (currentVelocity.Length() > currentDistanceToBody)
		{
			currentVelocity = currentVelocity.Normalized() * currentDistanceToBody;
		}

		if (currentDistanceToBody > MaxAllowedDistance)
		{
			CameraContainerNode.GlobalPosition += currentDir.Normalized() * (currentDistanceToBody - MaxAllowedDistance);
		}

		CameraContainerNode.GlobalPosition += currentVelocity;

		CameraPivotNode.Rotation = new Vector3(
			Mathf.LerpAngle(CameraPivotNode.Rotation.X, Mathf.DegToRad(targetRotation.X), CameraPanBlendSpeed * (float)delta),
			Mathf.LerpAngle(CameraPivotNode.Rotation.Y, Mathf.DegToRad(targetRotation.Y), CameraPanBlendSpeed * (float)delta),
			0);
    }
}
