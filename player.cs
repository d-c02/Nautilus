using Godot;
using System;
using System.Collections;
using System.Diagnostics;
using System.Security.AccessControl;

public partial class player : CharacterBody3D
{

    [Export]
    private AnimationTree _AnimationTree;

    private AnimationNodeStateMachinePlayback _AnimationNodeStateMachinePlayback;

    private AnimationNodeTimeScale _RunTimeScale;

    public bool InSpecialJumpTransition = false;
    public override void _Ready()
    {
        _AnimationNodeStateMachinePlayback = _AnimationTree.Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();
        //_RunTimeScale = _AnimationTree.Get("parameters/Run/TimeScale/scale").As<AnimationNodeTimeScale>();
    }
    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();
    }

    //TODO: Anim state stuff needs to be changed to stringnames instead of strings to optimize performance.
    public void SetAnimState(string state)
    {
        _AnimationNodeStateMachinePlayback.Travel(state);
    }

    public StringName GetAnimState()
    {
        return _AnimationNodeStateMachinePlayback.GetCurrentNode();
    }

    //Makes the run animation go faster or slower depending on player speed, but normalizes it so there are no extremes.
    public void SetRunTimeScale(float val)
    {
        //_RunTimeScale.Set("TimeScale", val);
        float normalizer = 10;
        float minRunSpeed = 0.2f;
        float maxRunSpeed = 2.0f;
        _AnimationTree.Set("parameters/Run/TimeScale/scale", Math.Min(Math.Max(val/normalizer, minRunSpeed), maxRunSpeed));
    }

    public CameraPivot GetCameraPivot()
    {
        return GetNode<CameraPivot>("CameraPivot");
    }
}
