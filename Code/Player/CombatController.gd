extends Node

signal OnCombatBegin
signal OnCombatEnd

@export var HandContainer: Node3D;
@export var HipContainer: Node3D;
@export var ItemContainer: Node3D;
@export var upperBodyStatePlaybackPath: String;
@export var oneHandStanceName: String;
@export var twoHandStanceName: String;

@export var animationTree: AnimationTree;

var isInCombat: bool;
var usingTwoHands: bool;

func _input(event):
	if Input.is_action_just_pressed("DrawWeapon"):
		var playback = animationTree.get(upperBodyStatePlaybackPath) as AnimationNodeStateMachinePlayback;
		if !isInCombat:
			isInCombat = true
			OnCombatBegin.emit()
			if usingTwoHands:
				playback.travel(twoHandStanceName + "Idle")
			else:
				playback.travel(oneHandStanceName + "Idle")
		else:
			isInCombat = false
			OnCombatEnd.emit()
			playback.travel("Idle")
	if isInCombat:
		if Input.is_action_just_pressed("SwapHands"):
			var playback = animationTree.get(upperBodyStatePlaybackPath) as AnimationNodeStateMachinePlayback;
			if usingTwoHands:
				playback.travel(oneHandStanceName + "Idle")
			else:
				playback.travel(twoHandStanceName + "Idle")
			usingTwoHands = !usingTwoHands
		
		if Input.is_action_just_pressed("UseWeapon"):
			var playback = animationTree.get(upperBodyStatePlaybackPath) as AnimationNodeStateMachinePlayback;
			if usingTwoHands:
				playback.travel(twoHandStanceName + "Attack1")
			else:
				playback.travel(oneHandStanceName + "Attack1")

func EquipWeapon():
	var parent = ItemContainer.get_parent()
	parent.remove_child(ItemContainer)
	HandContainer.add_child(ItemContainer)
	ItemContainer.position = Vector3.ZERO
	ItemContainer.rotation_degrees = Vector3.ZERO

func UnEquipWeapon():
	var parent = ItemContainer.get_parent()
	parent.remove_child(ItemContainer)
	HipContainer.add_child(ItemContainer)
	ItemContainer.position = Vector3.ZERO
	ItemContainer.rotation_degrees = Vector3.ZERO
