using Godot;
using System;

public partial class PlayerDiving : State
{

    [Export] private player _Player;

    [Export]
    public int IdleSpeed { get; set; } = 9;


    [Export]
    public int IdleAcceleration { get; set; } = 10;

    [Export]
    public int FallAcceleration { get; set; } = 100;

    [Export]
    public int FallImpulse = 50;

    [Export]
    private Camera3D _Camera;

    private Vector3 _targetVelocity;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void Enter()
    {
        _targetVelocity = new Vector3(_Player.Velocity.X, _Player.Velocity.Y - FallImpulse, _Player.Velocity.Z);
        _Player.FloorSnapLength = 0f;
        _Player._SetAnimState("Dive");
    }

    public override void Exit()
    {

    }

    public override void Update(double delta)
    {

    }

    public override void PhysicsUpdate(double delta)
    {
        var direction = Vector3.Zero;

        Vector3 cameraDifferenceVector = (_Player.GlobalPosition - _Camera.GlobalPosition).Normalized();
        cameraDifferenceVector.Y = 0;
        Vector3 orthogonalCameraDifferenceVector = new Vector3(-1 * cameraDifferenceVector.Z, 0, cameraDifferenceVector.X);

        if (Input.IsActionPressed("move_right"))
        {
            direction += orthogonalCameraDifferenceVector * Input.GetActionStrength("move_right"); //times GetActionStrength("move_right")
        }
        if (Input.IsActionPressed("move_left"))
        {
            direction -= orthogonalCameraDifferenceVector * Input.GetActionStrength("move_left");
        }
        if (Input.IsActionPressed("move_back"))
        {
            direction -= cameraDifferenceVector * Input.GetActionStrength("move_back");
        }
        if (Input.IsActionPressed("move_forward"))
        {
            direction += cameraDifferenceVector * Input.GetActionStrength("move_forward");
        }
        if (direction.Length() < 0.1)
        {
            direction = Vector3.Zero;
        }
        if (direction != Vector3.Zero && direction.Length() > 1)
        {
            direction = direction.Normalized();
        }

        //_targetVelocity.X = direction.X * IdleSpeed;
        //_targetVelocity.Z = direction.Z * IdleSpeed;

        Vector2 groundVelocity = new Vector2(direction.X * IdleSpeed, direction.Z * IdleSpeed);
        Vector2 playerVelocity = new Vector2(_Player.Velocity.X, _Player.Velocity.Z);
        if (Math.Abs(playerVelocity.Length() - groundVelocity.Length()) > 1)
        {
            if (playerVelocity.Length() > groundVelocity.Length())
            {
                Vector2 decelDirection = playerVelocity.Normalized();
                _targetVelocity.X = _Player.Velocity.X - (float)(IdleAcceleration * delta * decelDirection.X);

                //decelDirection.Y isn't an error, because it's equal to _Player.Velocity.Z.
                _targetVelocity.Z = _Player.Velocity.Z - (float)(IdleAcceleration * delta * decelDirection.Y);
            }
            else if (playerVelocity.Length() < groundVelocity.Length())
            {
                _targetVelocity.X = _Player.Velocity.X + (float)(IdleAcceleration * delta * direction.X);
                _targetVelocity.Z = _Player.Velocity.Z + (float)(IdleAcceleration * delta * direction.Z);
            }
        }

        float tmpAccel = FallAcceleration;
        _targetVelocity.Y = _targetVelocity.Y - (float)(tmpAccel * delta);

        if (_Player.IsOnFloor())
        {
            _targetVelocity.Y = 0;

        }
        _Player.Velocity = _targetVelocity;
        if (_Player.IsOnFloor())
        {
            EmitSignal(SignalName.Transitioned, this.Name + "", "Grounded");
        }
    }
}
