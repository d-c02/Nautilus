using Godot;
using System;

public partial class PlayerRolling : State
{
    [Export] private player _Player;

    [Export] private Node3D _Pivot;

    [Export]
	private CollisionShape3D _StandardCollider;

	[Export]
	private CollisionShape3D _RollingCollider;

    [Export]
    private double _ActiveTime = 0.4f;

    private double _CurTime = 0;

    [Export]
    private float _Speed = 35f;

    [Export]
    public int JumpSpeed { get; set; } = 20;

    [Export]
    private int Gravity { get; set; } = 50;

    [Export]
    public double AirDelay { get; set; } = 0.15; //How long you need to be in the air before going into falling state

    private double _CurAirTime;

    private Vector3 tmpVelocity = Vector3.Zero;

    [Export]
    private float _SlowdownConstant = 4.0f;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
    }

    public override void Enter()
    {
        //_StandardCollider.Disabled = true;
        //_RollingCollider.Disabled = false;
        //_Direction = _Pivot.GlobalTransform.Basis.Z.Normalized();
        Vector3 direction;
        Vector3 tmpDir = new Vector3(_Player.Velocity.X, 0, _Player.Velocity.Z);
        if (tmpDir.Length() < 0.1)
        {
            direction = _Pivot.GlobalTransform.Basis.Z.Normalized();
            direction.X *= -1;
            direction.Z *= -1;
        }
        else
        {
            direction = tmpDir.Normalized();
        }
        _Player.SetAnimState("Roll");
        tmpVelocity.X = direction.X * _Speed + _Player.Velocity.X / _SlowdownConstant;
        tmpVelocity.Z = direction.Z * _Speed + _Player.Velocity.Z / _SlowdownConstant;
        _CurTime = 0;
        _Player.GetNode<Node3D>("Pivot").LookAt(_Player.Position + direction, Vector3.Up);
    }

    public override void Exit()
    {
        //_StandardCollider.Disabled = false;
        //_RollingCollider.Disabled = true;
    }

    public override void Update(double delta)
    {

    }

    public override void PhysicsUpdate(double delta)
    {
        if (_CurTime > _ActiveTime && _Player.IsOnFloor() && !Input.IsActionJustPressed("jump"))
        {
            EmitSignal(SignalName.Transitioned, this.Name + "", "Grounded");
        }

        if (!_Player.IsOnFloor())
        {
            _CurAirTime += delta;
            tmpVelocity.Y -= Gravity * (float) delta;
        }
        else
        {
            _CurAirTime = 0;
            tmpVelocity.Y = 0;
        }
        if (Input.IsActionJustPressed("jump"))
        {
            tmpVelocity.Y = 0;
            _Player.Velocity = new Vector3(_Player.Velocity.X, JumpSpeed, _Player.Velocity.Z);
        }
        _Player.Velocity = new Vector3(tmpVelocity.X,_Player.Velocity.Y + tmpVelocity.Y,tmpVelocity.Z);
        if (Input.IsActionJustPressed("jump") || _CurAirTime > AirDelay)
        {
            EmitSignal(SignalName.Transitioned, this.Name + "", "Falling");
        }
        _CurTime += delta;
    }
}
