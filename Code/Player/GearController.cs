using Godot;
using System;

public partial class GearController : Node
{
	[Export] public PackedScene ArmorPrefab { get; set; }
	[Export] public Skeleton3D SkeletonNode { get; set; }

	private MeshInstance3D currentArmor;

	public override void _Input(InputEvent @event)
	{
		if (Input.IsActionJustPressed("EquipArmor"))
		{
			if (!IsInstanceValid(currentArmor))
			{
				currentArmor = ArmorPrefab.Instantiate() as MeshInstance3D;
				SkeletonNode.AddChild(currentArmor);
				currentArmor.Skeleton = SkeletonNode.GetPath();
			}
			else
			{
				currentArmor.QueueFree();
			}
		}
	}
}
