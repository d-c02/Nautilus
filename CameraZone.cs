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
    private player _Player;

    [Signal]
    public delegate void CameraZoneEnterEventHandler(CameraZone zone);

    [Signal]
    public delegate void CameraZoneExitEventHandler(CameraZone zone);

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

    private void OnAreaEntered(Node3D other)
    {
        if (other.Name == "Player")
		{
			//_Camera.MakeCurrent();
            EmitSignal(SignalName.CameraZoneEnter, this);
		}
    }

    private void OnAreaExited(Node3D other)
    {
        if (other.Name == "Player")
        {
            EmitSignal(SignalName.CameraZoneExit, this);
            //_Camera.ClearCurrent();
        }
    }
}
