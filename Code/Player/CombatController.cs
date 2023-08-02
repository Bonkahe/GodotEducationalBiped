using Godot;
using System;

public partial class CombatController : Node
{
    [Signal] public delegate void OnCombatBeginEventHandler();
    [Signal] public delegate void OnCombatEndEventHandler();
    [Export] public Node3D HandContainer { get; set; }
    [Export] public Node3D HipContainer { get; set; }
    [Export] public Node3D ItemContainer { get; set; }
    [Export] public string UpperBodyStatePlaybackPath { get; set; }
    [Export] public string OneHandStanceName { get; set; }
    [Export] public string TwoHandStanceName { get; set; }

    [Export] public AnimationTree animationTree { get; set; }


    private bool isInCombat;
    private bool usingTwoHands;

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("DrawWeapon"))
        {
            var playback = (AnimationNodeStateMachinePlayback)animationTree.Get(UpperBodyStatePlaybackPath);
            if (!isInCombat)
            {
                isInCombat = true;
                EmitSignal(SignalName.OnCombatBegin);
                if (usingTwoHands)
                {
                    playback.Travel(TwoHandStanceName + "Idle");
                }
                else
                {
                    playback.Travel(OneHandStanceName + "Idle");
                }
            }
            else
            {
                isInCombat = false;
                EmitSignal(SignalName.OnCombatEnd);
                playback.Travel("Idle");
            }
        }
        if (isInCombat)
        {
            if (Input.IsActionJustPressed("SwapHands"))
            {
                var playback = (AnimationNodeStateMachinePlayback)animationTree.Get(UpperBodyStatePlaybackPath);
                if (usingTwoHands)
                {
                    playback.Travel(OneHandStanceName + "Idle");
                }
                else
                {
                    playback.Travel(TwoHandStanceName + "Idle");
                }
                usingTwoHands = !usingTwoHands;
            }

            if (Input.IsActionJustPressed("UseWeapon"))
            {
                var playback = (AnimationNodeStateMachinePlayback)animationTree.Get(UpperBodyStatePlaybackPath);
                if (usingTwoHands)
                {
                    playback.Travel(TwoHandStanceName + "Attack1");
                }
                else
                {
                    playback.Travel(OneHandStanceName + "Attack1");
                }
            }
        }
    }

    public void EquipWeapon()
    {
        var parent = ItemContainer.GetParent();
        parent.RemoveChild(ItemContainer);
        HandContainer.AddChild(ItemContainer);
        ItemContainer.Position = Vector3.Zero;
        ItemContainer.RotationDegrees = Vector3.Zero;
    }

    public void UnEquipWeapon()
    {
        var parent = ItemContainer.GetParent();
        parent.RemoveChild(ItemContainer);
        HipContainer.AddChild(ItemContainer);
        ItemContainer.Position = Vector3.Zero;
        ItemContainer.RotationDegrees = Vector3.Zero;
    }
}
