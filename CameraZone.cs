using Godot;
using System;

public partial class CameraZone : Node3D
{

	[Export]
	private Area3D _Area;

	[Export]
	public Camera3D Camera;

    [Export]
    public Vector3 _MovementVector;

    [Export]
    protected player _Player;

    [Signal]
    public delegate void CameraZoneEnterEventHandler(CameraZone zone);

    [Signal]
    public delegate void CameraZoneExitEventHandler(CameraZone zone);

    public enum CameraZones { Fixed, OnTrack };

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _Area.Connect(Area3D.SignalName.BodyEntered, new Callable(this, CameraZone.MethodName.OnAreaEntered));
        _Area.Connect(Area3D.SignalName.BodyExited, new Callable(this, CameraZone.MethodName.OnAreaExited));
        this.Connect(CameraZone.SignalName.CameraZoneEnter, new Callable(_Player.GetCameraPivot(), CameraPivot.MethodName.CameraZoneEntered));
        this.Connect(CameraZone.SignalName.CameraZoneExit, new Callable(_Player.GetCameraPivot(), CameraPivot.MethodName.CameraZoneExit));
        _MovementVector = _MovementVector.Normalized();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

    }

    public override void _PhysicsProcess(double delta)
    {
        
    }
    public virtual int GetZoneType()
    {
        return (int) CameraZones.Fixed;
    }

    private void OnAreaEntered(Node3D other)
    {
        if (other.Name == "Player")
		{
            EmitSignal(SignalName.CameraZoneEnter, this);
		}
    }

    private void OnAreaExited(Node3D other)
    {
        if (other.Name == "Player")
        {
            EmitSignal(SignalName.CameraZoneExit, this);
        }
    }
}
