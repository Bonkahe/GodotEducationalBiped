extends Node

@export var armorPrefab: PackedScene;
@export var skeletonNode: Skeleton3D;

var currentArmor: MeshInstance3D;

func _input(event):
	if (Input.is_action_just_pressed("EquipArmor")):
		if (!is_instance_valid(currentArmor)):
			currentArmor = armorPrefab.instantiate() as MeshInstance3D
			skeletonNode.add_child(currentArmor)
			currentArmor.skeleton = skeletonNode.get_path()
		else:
			currentArmor.queue_free()
