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
    private double _ActiveTime = 0.2f;

    private double _CurTime = 0;

    [Export]
    private float _Acceleration = 4f;

    [Export]
    public int JumpSpeed { get; set; } = 20;

    [Export]
    private int Gravity { get; set; } = 50;

    [Export]
    public double AirDelay { get; set; } = 0.1; //How long you need to be in the air before going into falling state

    private double _CurAirTime;

    private Vector3 tmpVelocity = Vector3.Zero;

    private Vector3 _Direction;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
    }

    public override void Enter()
    {
		_StandardCollider.Disabled = true;
		_RollingCollider.Disabled = false;
        _Direction = _Pivot.GlobalTransform.Basis.Z.Normalized();

    }

    public override void Exit()
    {
        _StandardCollider.Disabled = false;
        _RollingCollider.Disabled = true;
        _CurTime = 0;
    }

    public override void Update(double delta)
    {

    }

    public override void PhysicsUpdate(double delta)
    {
        if (_CurTime > _ActiveTime)
        {
            EmitSignal(SignalName.Transitioned, this.Name + "", "Grounded");
        }
        //if (_CurTime >= _ActiveTime / 2)
        //{
        //tmpVelocity = _Direction * _Acceleration;
        //}
        //else
        //{
        //tmpVelocity = -1 * _Direction * _Acceleration;
        tmpVelocity.X = -1 * _Direction.X * _Acceleration;
        tmpVelocity.Z = -1 * _Direction.Z * _Acceleration;
        //}
        //tmpVelocity.X += _Player.GetFloorNormal().X * _Acceleration * (float) delta;
        //tmpVelocity.Z += _Player.GetFloorNormal().Z * _Acceleration * (float) delta;
        if (!_Player.IsOnFloor())
        {
            _CurAirTime += delta;
            tmpVelocity.Y -= -1 * Gravity * (float) delta;
        }
        else
        {
            _CurAirTime = 0;
            tmpVelocity.Y = 0;
        }
        if (Input.IsActionJustPressed("jump"))
        {
            tmpVelocity.Y = JumpSpeed;
        }
        _Player.Velocity += tmpVelocity;
        if (Input.IsActionJustPressed("jump") || _CurAirTime > AirDelay)
        {
            EmitSignal(SignalName.Transitioned, this.Name + "", "Falling");
        }
        _CurTime += delta;
    }
}
