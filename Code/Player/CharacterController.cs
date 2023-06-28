using Godot;
using System;

public partial class CharacterController : CharacterBody3D
{
	[Export] public string LocomotionBlendPath { get; set; }
	[Export] public AnimationTree animationTree { get; set; }
	[Export] public float TransitionSpeed { get; set; } = 0.1f;
	[Export] public float Speed { get; set; } = 5.0f;
    [Export] public float JumpVelocity { get; set; } = 4.5f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	private Vector2 currentInput = Vector2.Zero;
    private Vector2 currentVelocity = Vector2.Zero;

	public override void _Process(double delta)
	{
		Vector2 newDelta = currentInput - currentVelocity;
		if (newDelta.Length() > TransitionSpeed * (float)delta)
		{
			newDelta = newDelta.Normalized() * TransitionSpeed * (float)delta;
        }
		currentVelocity += newDelta;

		animationTree.Set(LocomotionBlendPath, currentVelocity);

    }

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        currentInput = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector3 direction = (Transform.Basis * new Vector3(currentInput.X, 0, currentInput.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
