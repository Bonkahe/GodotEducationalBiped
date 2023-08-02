using Godot;
using System;

public partial class CharacterController : CharacterBody3D
{
	[Export] public Node3D CameraContainer { get; set; }

    [Export] public string LocomotionStatePlaybackPath { get; set; }
	[Export] public string LocomotionBlendPath { get; set; }
	[Export] public string JumpStateName { get; set; }
    [Export] public string FallingStateName { get; set; }
    [Export] public string WalkingStateName { get; set; }

	[Export] public AnimationTree animationTree { get; set; }
	[Export] public float TransitionSpeed { get; set; } = 0.1f;
	[Export] public float Speed { get; set; } = 5.0f;
	[Export] public float RotationSpeed { get; set; } = 10;
    [Export] public float JumpVelocity { get; set; } = 4.5f;

	public bool allowVelocityRotation = true;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	private Vector2 currentInput = Vector2.Zero;
    private Vector2 currentVelocity = Vector2.Zero;

	private bool jumpQueued = false;
	private bool falling = false;

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

	public override void _Input(InputEvent @event)
	{
		if (IsOnFloor() && Input.IsActionJustPressed("ui_accept"))
		{
            BeginJump();
        }
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		if (!IsOnFloor())
		{
			velocity.Y -= gravity * (float)delta;
			jumpQueued = false;
			if (!falling)
			{
				falling = true;
                var playback = (AnimationNodeStateMachinePlayback)animationTree.Get(LocomotionStatePlaybackPath);
                playback.Travel(FallingStateName);
            }
		}
		else if (falling)
		{
			falling = false;
            var playback = (AnimationNodeStateMachinePlayback)animationTree.Get(LocomotionStatePlaybackPath);
            playback.Travel(WalkingStateName);
        }

		// Putting it after the falling handler makes sure that the transition doesn't
		// automatically force it into a falling animation instead of letting the jump animation
		// naturally finish.

		if (jumpQueued)
		{
			velocity.Y = JumpVelocity;
			jumpQueued = false;
			falling = true;
		}

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        currentInput = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector3 direction = (CameraContainer.Transform.Basis * new Vector3(currentInput.X, 0, currentInput.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
			Vector3 currentNormalizedVelocity = ToLocal(GlobalPosition + velocity).Normalized();
			currentInput = new Vector2(currentNormalizedVelocity.X, currentNormalizedVelocity.Z);
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);

			currentInput = Vector2.Zero;
		}

		Velocity = velocity;

		if (allowVelocityRotation)
		{
			if (Velocity.Length() > 0.1f)
			{
				RotationDegrees = new Vector3(
					RotationDegrees.X,
					Mathf.RadToDeg(Mathf.LerpAngle(Mathf.DegToRad(RotationDegrees.Y), Mathf.Atan2(-Velocity.X, -Velocity.Z), (float)delta * RotationSpeed)),
					RotationDegrees.Z
					);
			}
		}
		else
		{
            RotationDegrees = new Vector3(
                    RotationDegrees.X,
                    CameraContainer.RotationDegrees.Y,
                    RotationDegrees.Z
                    );
        }

		MoveAndSlide();
	}

	public void DisableVelocityRotation()
	{
		allowVelocityRotation = false;
	}

	public void EnableVelocityRotation()
	{
		allowVelocityRotation = true;
	}

	private void BeginJump()
	{
		var playback = (AnimationNodeStateMachinePlayback)animationTree.Get(LocomotionStatePlaybackPath);
		playback.Travel(JumpStateName);
	}

	public void ExecuteJumpVelocity()
	{
		jumpQueued = true;
	}
}
