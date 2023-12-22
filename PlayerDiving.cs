using Godot;
using System;

public partial class PlayerDiving : State
{

    [Export] private player _Player;

    [Export]
    public int IdleSpeed { get; set; } = 20;


    [Export]
    public int IdleAcceleration { get; set; } = 10;

    [Export]
    public int FallAcceleration { get; set; } = 100;

    [Export]
    public int FallImpulse = 20;

    [Export]
    private Camera3D _Camera;

    private Vector3 _targetVelocity;

    [Export]
    public double _FloatTime = 0.3;

    private double _CurFloatTime;

    private float _SpeedEntered;

    private float _SlowdownConstant = 1.0f;

    private bool _IsFalling = false;
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
        _SpeedEntered = new Vector3(_Player.Velocity.X, 0, _Player.Velocity.Z).Length();
        _SpeedEntered /= _SlowdownConstant;
        _Player.FloorSnapLength = 0f;
        _Player._SetAnimState("Dive");
        _IsFalling = false;
    }

    public override void Exit()
    {
        _CurFloatTime = 0;
        _Player.FloorSnapLength = 0.85f;
    }

    public override void Update(double delta)
    {

    }

    public override void PhysicsUpdate(double delta)
    {
        var direction = Vector3.Zero;

        Vector3 cameraDifferenceVector = (_Player.GlobalPosition - _Camera.GlobalPosition);
        cameraDifferenceVector.Y = 0;
        cameraDifferenceVector = cameraDifferenceVector.Normalized();
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

        _targetVelocity.X = direction.X * IdleSpeed + direction.X * _SpeedEntered;
        _targetVelocity.Z = direction.Z * IdleSpeed + direction.Z * _SpeedEntered;
        float tmpAccel = FallAcceleration;
        _targetVelocity.Y = _targetVelocity.Y - (float)(tmpAccel * delta);

        if (_Player.IsOnFloor())
        {
            _targetVelocity.Y = 0;

        }
        if (_CurFloatTime < _FloatTime)
        {
            _Player.Velocity = Vector3.Zero;
        }
        else if (!_IsFalling)
        {
            _Player.Velocity = _targetVelocity;
            _IsFalling = true;
        }
        else
        {

        }
        _CurFloatTime += delta;
        if (_Player.IsOnFloor())
        {
            //_Player.Velocity = new Vector3(_Player.Velocity.X, 0, _Player.Velocity.Z);
            _Player.Velocity = Vector3.Zero;
            EmitSignal(SignalName.Transitioned, this.Name + "", "Grounded");
        }
    }
}
