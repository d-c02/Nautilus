using Godot;
using System;
using System.Diagnostics;

public partial class OnTrackCameraZone : CameraZone
{
    [Export]
    private Path3D _Path;

    [Export]
    private Node3D _CameraPivot;

    //[Export]
    //private float _InterpolationSpeed = 100.0f;

    public override void _Ready()
    {
        base._Ready();
    }
    public override int GetZoneType()
    {
        return (int)CameraZones.OnTrack;
    }

    public override void _PhysicsProcess(double delta)
    {
        float playerOffset = _Path.Curve.GetClosestOffset(_Path.ToLocal(_Player.GlobalPosition));
        //_CameraPivot.GlobalPosition = _CameraPivot.GlobalPosition.Lerp(_Path.ToGlobal(-1 * _Path.Curve.SampleBaked(playerOffset)), (float) delta * _InterpolationSpeed);

        //_CameraPivot.GlobalPosition = _Path.ToGlobal(_Path.Curve.SampleBaked(playerOffset));
        _CameraPivot.GlobalPosition = _Path.ToGlobal(_Path.Curve.SampleBaked(playerOffset));
    }

}
