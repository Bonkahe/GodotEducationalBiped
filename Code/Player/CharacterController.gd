extends CharacterBody3D

@export var cameraContainer: Node3D;

@export var locomotionStatePlaybackPath: String;
@export var locomotionBlendPath: String;
@export var jumpStateName: String;
@export var fallingStateName: String;
@export var walkingStateName: String;

@export var animationTree: AnimationTree;
@export var transitionSpeed: float = 0.1;
@export var speed: float = 5.0
@export var rotationSpeed: float = 10;
@export var jumpVelocity: float = 4.5

var allowVelocityRotation: bool = true;
# Get the gravity from the project settings to be synced with RigidBody nodes.
var gravity = ProjectSettings.get_setting("physics/3d/default_gravity")

var currentInput: Vector2;
var currentVelocity: Vector2;

var jumpQueued: bool;
var falling: bool;

func _process(delta):
	
	var newDelta = currentInput - currentVelocity;
	if (newDelta.length() > transitionSpeed * delta):
		newDelta = newDelta * transitionSpeed * delta;
	
	currentVelocity += newDelta;
	
	animationTree.set(locomotionBlendPath, currentVelocity);

func _input(event):
	if is_on_floor() && Input.is_action_just_pressed("ui_accept"):
		BeginJump()

func _physics_process(delta):
	
	if !is_on_floor():
		velocity.y -= gravity * delta
		jumpQueued = false
		if !falling:
			falling = true
			var playback = animationTree.get(locomotionStatePlaybackPath) as AnimationNodeStateMachinePlayback
			playback.travel(fallingStateName)
	else: if falling:
		falling = false
		var playback = animationTree.get(locomotionStatePlaybackPath) as AnimationNodeStateMachinePlayback
		playback.travel(walkingStateName)
	
	# Putting it after the falling handler makes sure that the transition doesn't
	# automatically force it into a falling animation instead of letting the jump animation
	# naturally finish.
	
	if jumpQueued:
		velocity.y = jumpVelocity
		jumpQueued = false
		falling = true

	# Get the input direction and handle the movement/deceleration.
	# As good practice, you should replace UI actions with custom gameplay actions.
	currentInput = Input.get_vector("ui_left", "ui_right", "ui_up", "ui_down")
	var direction = (cameraContainer.transform.basis * Vector3(currentInput.x, 0, currentInput.y)).normalized()
	if direction:
		velocity.x = direction.x * speed
		velocity.z = direction.z * speed
		var currentNormalizedVelocity = to_local(global_position + velocity)
		currentInput = Vector2(currentNormalizedVelocity.x, currentNormalizedVelocity.z).limit_length(1)
	else:
		velocity.x = move_toward(velocity.x, 0, speed)
		velocity.z = move_toward(velocity.z, 0, speed)
		currentInput = Vector2.ZERO
	
	if (allowVelocityRotation):
		if (velocity.length() > 0.1):
			rotation_degrees.y = rad_to_deg(lerp_angle(deg_to_rad(rotation_degrees.y), atan2(-velocity.x, -velocity.z), delta * rotationSpeed))
	else:
		rotation_degrees.y = cameraContainer.rotation_degrees.y;
	
	move_and_slide()

func DisableVelocityRotation():
	allowVelocityRotation = false

func EnableVelocityRotation():
	allowVelocityRotation = true

func BeginJump():
	var playback = animationTree.get(locomotionStatePlaybackPath) as AnimationNodeStateMachinePlayback
	playback.travel(jumpStateName)

func ExecuteJumpVelocity():
	jumpQueued = true;
