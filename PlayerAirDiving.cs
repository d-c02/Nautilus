using Godot;
using System;
using System.IO;

public partial class PlayerAirDiving : State
{

    [Export]
    private float _DiveImpulse { get; set; } = 50.0f;

    [Export]
    private int _FallAcceleration { get; set; } = 100;

    [Export]
    private player _Player;

    [Export] 
    private Node3D _Pivot;

    [Export]
    private CameraPivot _Camera;

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
        var direction = Vector3.Zero;
        Vector3 cameraDifferenceVector = _Camera.GetMovementVector(_Player.GlobalPosition);
        cameraDifferenceVector.Y = 0;
        cameraDifferenceVector = cameraDifferenceVector.Normalized();
        Vector3 orthogonalCameraDifferenceVector = new Vector3(-1 * cameraDifferenceVector.Z, 0, cameraDifferenceVector.X);


        if (Input.IsActionPressed("move_right") && Input.GetActionStrength("move_right") > 0.1)
        {
            direction += orthogonalCameraDifferenceVector * Input.GetActionStrength("move_right"); //times GetActionStrength("move_right")
        }
        if (Input.IsActionPressed("move_left") && Input.GetActionStrength("move_left") > 0.1)
        {
            direction -= orthogonalCameraDifferenceVector * Input.GetActionStrength("move_left");
        }
        if (Input.IsActionPressed("move_back") && Input.GetActionStrength("move_back") > 0.1)
        {
            direction -= cameraDifferenceVector * Input.GetActionStrength("move_back");
        }
        if (Input.IsActionPressed("move_forward") && Input.GetActionStrength("move_forward") > 0.1)
        {
            direction += cameraDifferenceVector * Input.GetActionStrength("move_forward");
        }
        if (direction.Length() < 0.1)
        {
            direction = _Pivot.GlobalTransform.Basis.Z.Normalized();
            direction.X *= -1;
            direction.Z *= -1;
        }
        if (direction != Vector3.Zero && direction.Length() > 1)
        {
            direction = direction.Normalized();
        }

        _Player.Velocity = direction * _DiveImpulse;
        _Player.GetNode<Node3D>("Pivot").LookAt(_Player.Position + direction, Vector3.Up);
        _Player.SetAnimState("AirDive");
    }

    public override void Exit()
    {
        Vector3 direction = _Pivot.GlobalTransform.Basis.Z.Normalized();
        direction.Y = 0;
        _Player.GetNode<Node3D>("Pivot").LookAt(_Player.Position - direction, Vector3.Up);
    }

    public override void Update(double delta)
    {
        
    }

    public override void PhysicsUpdate(double delta)
    {
        _targetVelocity = _Player.Velocity;

        _targetVelocity.Y -= _FallAcceleration * (float) delta;

        _Player.Velocity = _targetVelocity;

        //if (!_Player.Position.IsEqualApprox(_Player.Position + _Player.Velocity))
        //{
        //    _Player.GetNode<Node3D>("Pivot").LookAt(_Player.Position + _Player.Velocity, Vector3.Up);
        //}

        if (!_Player.Velocity.IsZeroApprox() && !_Player.Velocity.Normalized().IsEqualApprox(Vector3.Up) && !_Player.Velocity.Normalized().IsEqualApprox(Vector3.Down))
        {
            _Player.GetNode<Node3D>("Pivot").LookAt(_Player.Position + _Player.Velocity, Vector3.Up);
        }
        if (_Player.IsOnFloor())
        {
            _Player.Velocity = Vector3.Zero;
            EmitSignal(SignalName.Transitioned, this.Name + "", "Grounded");
        }


    }
}
