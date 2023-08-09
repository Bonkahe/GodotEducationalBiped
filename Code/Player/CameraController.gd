extends Node

@export var cameraContainerNode:Node3D;
@export var cameraPivotNode:Node3D;
@export var bodyNode:Node3D;

@export var cameraPanDesiredVerticalSpeed:float;
@export var cameraPanDesiredHorizontalSpeed:float;
@export var cameraPanBlendSpeed:float;

@export var cameraHorizontalMoveSpeed:float;
@export var cameraVerticalMoveSpeed:float;
@export var cameraMovementDampening:float;
@export var cameraChangeSpeed:float;
@export var maxAllowedDistance:float;

@export var cameraNode:Camera3D;
@export var cameraCollisionRay:RayCast3D;
@export var cameraZoomDesiredSpeed:float;
@export var cameraZoomBlendSpeed:float;
@export var cameraFastZoomBlendSpeed:float;

@export var minimumZoomDistance:float;
@export var maximumZoomDistance:float;
@export var raycastImpactOffset:float;

var targetRotation:Vector3;
var currentVelocity:Vector3;

var currentZoomDistance:float;

func _ready():
	Input.mouse_mode = Input.MOUSE_MODE_CAPTURED
	
	currentZoomDistance = minimumZoomDistance


func _input(event):
	if (event is InputEventMouseMotion):
		var delta = event.relative;
		
		targetRotation.y = wrap((-1 * delta.x * cameraPanDesiredHorizontalSpeed) + targetRotation.y, 0, 360)
		targetRotation.x = (-1 * delta.y * cameraPanDesiredVerticalSpeed) + cameraPivotNode.rotation_degrees.x
		
		targetRotation.x = clamp(targetRotation.x, -60, 50)
		
	
	if (event.is_action_pressed("ui_cancel")):
		if (Input.mouse_mode == Input.MOUSE_MODE_CAPTURED):
			Input.mouse_mode = Input.MOUSE_MODE_VISIBLE;
		else:
			Input.mouse_mode = Input.MOUSE_MODE_CAPTURED;
	
	if (event is InputEventMouseButton):
		if (event.button_index == MOUSE_BUTTON_WHEEL_UP):
			currentZoomDistance = clamp(currentZoomDistance - cameraZoomDesiredSpeed, minimumZoomDistance, maximumZoomDistance)
		if (event.button_index == MOUSE_BUTTON_WHEEL_DOWN):
			currentZoomDistance = clamp(currentZoomDistance + cameraZoomDesiredSpeed, minimumZoomDistance, maximumZoomDistance)

func _physics_process(delta):
	var currentDir = (bodyNode.global_position - cameraContainerNode.global_position).normalized()
	currentDir.x *= cameraHorizontalMoveSpeed
	currentDir.y *= cameraVerticalMoveSpeed
	currentDir.z *= cameraHorizontalMoveSpeed
	
	currentVelocity += (currentDir - currentVelocity) * delta * cameraChangeSpeed
	currentVelocity *= cameraMovementDampening
	
	var currentDistanceToBody = cameraContainerNode.global_position.distance_to(bodyNode.global_position)
	
	if (currentVelocity.length() > currentDistanceToBody):
		currentVelocity = currentVelocity.normalized() * currentDistanceToBody
	
	if (currentDistanceToBody > maxAllowedDistance):
		cameraContainerNode.global_position += currentDir.normalized() * (currentDistanceToBody - maxAllowedDistance)
	
	cameraContainerNode.global_position += currentVelocity
	
	cameraPivotNode.rotation = Vector3(
		lerp_angle(cameraPivotNode.rotation.x, deg_to_rad(targetRotation.x), cameraPanBlendSpeed * delta),
		lerp_angle(cameraPivotNode.rotation.y, deg_to_rad(targetRotation.y), cameraPanBlendSpeed * delta),
		0)
	
	cameraCollisionRay.target_position = Vector3(0,0,currentZoomDistance)
	
	if (cameraCollisionRay.is_colliding()):
		var targetPosition = cameraCollisionRay.get_collision_point() + (cameraCollisionRay.get_collision_normal() * raycastImpactOffset)
		
		if (targetPosition.distance_to(cameraCollisionRay.global_position) < cameraNode.global_position.distance_to(cameraCollisionRay.global_position) - raycastImpactOffset):
			cameraNode.global_position = cameraNode.global_position.lerp(targetPosition, cameraFastZoomBlendSpeed * delta)
		else:
			cameraNode.global_position = cameraNode.global_position.lerp(targetPosition, cameraZoomBlendSpeed * delta)
	else:
		cameraNode.global_position = cameraNode.global_position.lerp(cameraCollisionRay.to_global(cameraCollisionRay.target_position), cameraZoomBlendSpeed * delta)
	














