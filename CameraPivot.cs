using Godot;
using System;

public partial class CameraPivot : Node3D
{

    private Vector3 _targetRotation;

    [Export]
    public float RotationSpeed { get; set; } = 0.01f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _targetRotation = Rotation;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        var direction = Vector2.Zero;

        if (Input.IsActionPressed("tilt_camera_right"))
        {
            direction.Y += 1.0f;//times GetActionStrength("tilt_camera_right")
        }
        if (Input.IsActionPressed("tilt_camera_left"))
        {
            direction.Y -= 1.0f;
        }
        if (Input.IsActionPressed("tilt_camera_up"))
        {
            direction.X += 1.0f;
        }
        if (Input.IsActionPressed("tilt_camera_down"))
        {
            direction.X -= 1.0f;
        }
        if (direction != Vector2.Zero)
        {
            direction = direction.Normalized();
        }

        _targetRotation.X = _targetRotation.X + direction.X * RotationSpeed;
        _targetRotation.Y = _targetRotation.Y + direction.Y * RotationSpeed;
        Rotation = _targetRotation;
    }
}
