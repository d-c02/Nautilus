using Godot;
using System;

public partial class OnTrackCameraZone : CameraZone
{
    [Export]
    private Path3D _Path;

    [Export]
    private Node3D _CameraPivot;

    public override int GetZoneType()
    {
        return (int)CameraZones.OnTrack;
    }

    public override void _Process(double delta)
    {
        float playerOffset = _Path.Curve.GetClosestOffset(_Player.ToLocal(_Path.GlobalPosition));
        _CameraPivot.GlobalPosition = _Path.ToGlobal(-1 * _Path.Curve.SampleBaked(playerOffset));
    }

}
