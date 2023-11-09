using Godot;
using System;

public partial class PlayerRolling : State
{
    [Export] private CharacterBody3D _Player;

    [Export] private Node3D _Pivot;

    [Export]
	private CollisionShape3D _StandardCollider;

	[Export]
	private CollisionShape3D _RollingCollider;

    [Export]
    private double _ActiveTime = 0.1f;

    private double _CurTime = 0;

    [Export]
    private float _Acceleration = 8f;

    [Export]
    public int JumpSpeed { get; set; } = 20;


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
        Vector3 tmpVelocity = Vector3.Zero;
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
        tmpVelocity = -1 * _Direction * _Acceleration;
        //}
        tmpVelocity.X += _Player.GetFloorNormal().X * _Acceleration * (float) delta;
        tmpVelocity.Z += _Player.GetFloorNormal().Z * _Acceleration * (float) delta;
        if (Input.IsActionJustPressed("jump"))
        {
            tmpVelocity.Y = JumpSpeed;
        }
        _Player.Velocity += tmpVelocity;
        if (Input.IsActionJustPressed("jump") || !_Player.IsOnFloor())
        {
            EmitSignal(SignalName.Transitioned, this.Name + "", "Falling");
        }
        _CurTime += delta;
    }
}
