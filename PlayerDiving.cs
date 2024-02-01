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
    private CameraPivot _Camera;

    private Vector3 _targetVelocity;

    [Export]
    public double FloatTime = 0.3;

    private double _CurFloatTime;

    private float _SpeedEntered;

    private bool _IsFalling = false;

    [Export]
    private double _GrabTime = 0.2;

    [Export]
    private double _GrabCooldown = 0.5; 

    private double _CurGrabTime;

    private bool _IsRollGrabbing;

    private bool _IsJumpGrabbing;

    private bool _GrabBuffer = false;

    [Export]
    private int _JumpImpulse = 35;

    [Export]
    private float _SlowdownConstant = 6.0f;
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
        _Player.FloorSnapLength = 0f;
        _CurGrabTime = 0;
        _Player.SetAnimState("Dive");
        _IsFalling = false;
        _IsRollGrabbing = false;
        _IsJumpGrabbing = false;
        _GrabBuffer = false;
    }

    public override void Exit()
    {
        _CurFloatTime = 0;
        _Player.FloorSnapLength = 0.85f;
    }

    public override void Update(double delta)
    {

    }

    //TODO: Move input stuff to Update()
    public override void PhysicsUpdate(double delta)
    {
        var direction = Vector3.Zero;

        Vector3 cameraDifferenceVector = _Camera.GetMovementVector(_Player.GlobalPosition);
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

        if (_CurFloatTime < FloatTime)
        {
            _Player.Velocity = Vector3.Zero;
        }
        else if (!_IsFalling)
        {
            _Player.Velocity = _targetVelocity;
            _IsFalling = true;
        }

        if (Input.IsActionJustPressed("roll") && _GrabBuffer && !_IsJumpGrabbing)
        {
            _IsRollGrabbing = true;
        }
        if (Input.IsActionJustPressed("jump") && _GrabBuffer && !_IsRollGrabbing)
        {
            _IsJumpGrabbing = true;
        }
        if (_IsRollGrabbing || _IsJumpGrabbing)
        {
            _CurGrabTime += delta;
        }
        if (_CurGrabTime > _GrabCooldown)
        {
            _IsRollGrabbing = false;
            _IsJumpGrabbing = false;
            _CurGrabTime = 0;
        }
        _CurFloatTime += delta;
        _GrabBuffer = true;
        if (_Player.IsOnFloor())
        {
            //_Player.Velocity = new Vector3(_Player.Velocity.X, 0, _Player.Velocity.Z);
            if (_IsRollGrabbing && _CurGrabTime < _GrabTime)
            {
                _Player.Velocity = new Vector3(_Player.Velocity.X, 0, _Player.Velocity.Z);
                EmitSignal(SignalName.Transitioned, this.Name + "", "Rolling");
            }
            else if (_IsJumpGrabbing && _CurGrabTime < _GrabTime)
            {
                _Player.Velocity = new Vector3(_Player.Velocity.X / _SlowdownConstant, _JumpImpulse, _Player.Velocity.Z / _SlowdownConstant);
                _Player.SetAnimState("TallJumpTransition");
                _Player.InSpecialJumpTransition = true;
                EmitSignal(SignalName.Transitioned, this.Name + "", "Falling");
            }
            else
            {
                _Player.Velocity = Vector3.Zero;
                EmitSignal(SignalName.Transitioned, this.Name + "", "Grounded");
            }
        }
    }
}
