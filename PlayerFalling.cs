using Godot;
using Godot.NativeInterop;
using System;

public partial class PlayerFalling : State
{

	[Export] private player _Player;

	private bool floatFall = true;

	[Export]
	public int IdleSpeed { get; set; } = 14;


	[Export]
	public int IdleAcceleration { get; set; } = 25;

	[Export]
	public int FallAcceleration { get; set; } = 50;

	[Export]
	private CameraPivot _Camera;

	private Vector3 _targetVelocity;

	private bool _InJumpTransition;
	public override void _Ready()
	{

	}

	public override void Enter()
	{
		_targetVelocity = _Player.Velocity;
		if (!Input.IsActionPressed("jump"))
		{
			floatFall = false;
			_InJumpTransition = false;
		}
		else
		{
			floatFall = true;
			_InJumpTransition = true;
		}
		_Player.FloorSnapLength = 0f;
		Vector3 PVelocityXZ = new Vector3(_Player.Velocity.X, 0, _Player.Velocity.Z);
		if (PVelocityXZ > Vector3.Zero)
		{
			_Player.GetNode<Node3D>("Pivot").LookAt(_Player.Position + PVelocityXZ, Vector3.Up);
		}
	}

	public override void Exit()
	{
		_Player.FloorSnapLength = 0.85f;
		_Player.InSpecialJumpTransition = false;
	}

	public override void Update(double delta)
	{
		if (!Input.IsActionPressed("jump"))
		{
			floatFall = false;
		}

		if (Input.IsActionJustPressed("dive"))
		{
			//CHANGE BACK TO DIVING AND ADD SEPARATE AIRDIVE
			EmitSignal(SignalName.Transitioned, this.Name + "", "Diving");
		}

		if (Input.IsActionJustPressed("airdive"))
		{
			EmitSignal(SignalName.Transitioned, this.Name + "", "AirDive");
		}
	}

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
		if (!floatFall)
		{
			tmpAccel = FallAcceleration * 2;
		}
		_targetVelocity.Y = _targetVelocity.Y - (float)(tmpAccel * delta);

		if (_Player.IsOnFloor())
		{
			_targetVelocity.Y = 0;
		}
		if (!_Player.InSpecialJumpTransition)
		{
			if (_targetVelocity.Y > 0)
			{

				_Player.SetAnimState("Jump");
			}
			else
			{
				_Player.SetAnimState("JumpTransition");
			}
		}
		_Player.Velocity = _targetVelocity;
		if (_Player.IsOnFloor())
		{
			EmitSignal(SignalName.Transitioned, this.Name + "", "Grounded");
		}

		if (_Player.IsOnWall())
		{
			EmitSignal(SignalName.Transitioned, this.Name + "", "WallSlide");
		}
	}
}
