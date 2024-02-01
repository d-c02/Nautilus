using Godot;
using System;

public partial class PlayerWallSliding : State
{

    [Export]
    private int _FallAcceleration { get; set; } = 25;

    [Export]
    private int _JumpSpeed { get; set; } = 20;

    [Export]
    private float _VerticalExitSpeed { get; set; } = 25;

    [Export]
    private player _Player;

    private Vector3 _WallNormal;

    private Vector3 _TargetVelocity;

    [Export]
    private CameraPivot _Camera;

    private bool _Jumping = false;

    public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

    public override void Enter()
    {
        _Player.SetAnimState("WallSlide");
        _WallNormal = _Player.GetWallNormal();
        _Player.GetNode<Node3D>("Pivot").LookAt(_Player.Position - _WallNormal, Vector3.Up);
        _Player.Velocity = -_WallNormal;
        _TargetVelocity = -_WallNormal;
        _Jumping = false;
    }

    public override void Exit()
    {

    }

    public override void Update(double delta)
    {
        if (Input.IsActionJustPressed("jump"))
        {
            _Jumping = true;
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        _TargetVelocity.Y -= _FallAcceleration * (float) delta;

        _Player.Velocity = _TargetVelocity;

        //Included to prevent unneccessary debugger output. May be worth taking a look at.
        if (!_Player.Position.IsEqualApprox(_Player.Position - _Player.GetWallNormal()))
        {
            _Player.GetNode<Node3D>("Pivot").LookAt(_Player.Position - _Player.GetWallNormal(), Vector3.Up);
        }

        if (_Jumping)
        {
            Vector3 ExitVelocity = _VerticalExitSpeed * _WallNormal;
            ExitVelocity.Y = _JumpSpeed;
            _Player.Velocity = ExitVelocity;
            _Player.GetNode<Node3D>("Pivot").LookAt(_Player.Position + new Vector3(_WallNormal.X, 0, _WallNormal.Z), Vector3.Up);
            //_Player.SetAnimState("WallJumpTransition");
            _Player.SetAnimState("WallJumpTransition");
            _Player.InSpecialJumpTransition = true;
            EmitSignal(SignalName.Transitioned, this.Name + "", "Falling");
        }
        if (_Player.IsOnFloor())
        {
            _Player.Velocity = Vector3.Zero;
            _Player.GetNode<Node3D>("Pivot").LookAt(_Player.Position + new Vector3(_WallNormal.X, 0, _WallNormal.Z), Vector3.Up);
            EmitSignal(SignalName.Transitioned, this.Name + "", "Grounded");
        }
        else if (!_Player.IsOnWall())
        {
            _Player.Velocity = new Vector3(0, _Player.Velocity.Y, 0);
            EmitSignal(SignalName.Transitioned, this.Name + "", "Falling");
        }
    }
}
