extends Node3D

@export var bodyNode:Node3D;
@export var cameraPanSpeed:float;


func _ready():
	Input.mouse_mode = Input.MOUSE_MODE_CAPTURED;


func _input(event):
	if (event is InputEventMouseMotion):
		var delta = event.relative;
		
		var newY = bodyNode.rotation_degrees.y + (-delta.x * cameraPanSpeed);
		var newX = rotation_degrees.x + (delta.y * cameraPanSpeed);
		
		newX = clampf(newX, -60, 50);
		
		bodyNode.rotation_degrees = Vector3(0, newY, 0);
		
		rotation_degrees = Vector3(newX, rotation_degrees.y, 0);
		
	
	if (event.is_action_pressed("ui_cancel")):
		if (Input.mouse_mode == Input.MOUSE_MODE_CAPTURED):
			Input.mouse_mode = Input.MOUSE_MODE_VISIBLE;
		else:
			Input.mouse_mode = Input.MOUSE_MODE_CAPTURED;
