using Godot;
using System;

public partial class PlayerGrounded : State
{
    [Export] private CharacterBody3D _Player;

    [Export]
    public int IdleSpeed { get; set; } = 14;

    [Export]
    public int IdleAcceleration { get; set; } = 200;

    [Export]
    public int JumpSpeed { get; set; } = 20;

    private Vector3 _targetVelocity;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    // Called when the node enters the scene tree for the first time.
    public override void PhysicsUpdate(double delta)
    {
        var direction = Vector3.Zero;

        if (Input.IsActionPressed("move_right"))
        {
            direction.X += 1.0f; //times GetActionStrength("move_right")
        }
        if (Input.IsActionPressed("move_left"))
        {
            direction.X -= 1.0f;
        }
        if (Input.IsActionPressed("move_back"))
        {
            direction.Z += 1.0f;
        }
        if (Input.IsActionPressed("move_forward"))
        {
            direction.Z -= 1.0f;
        }
        if (Input.IsActionJustPressed("jump"))
        {
            _targetVelocity.Y = JumpSpeed;
        }

        if (direction != Vector3.Zero)
        {
            direction = direction.Normalized();
            _Player.GetNode<Node3D>("Pivot").LookAt(_Player.Position + direction, Vector3.Up);
        }


        _targetVelocity.X = direction.X * IdleSpeed;
        _targetVelocity.Z = direction.Z * IdleSpeed;

        Vector2 groundVelocity = new Vector2(_targetVelocity.X, _targetVelocity.Z);
        Vector2 playerVelocity = new Vector2(_Player.Velocity.X, _Player.Velocity.Z);
        if (Math.Abs(playerVelocity.Length() - groundVelocity.Length()) > 10)
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
        _Player.Velocity = _targetVelocity;

        if (!_Player.IsOnFloor() || Input.IsActionJustPressed("jump")) // Gravity
        {
            EmitSignal(SignalName.Transitioned, this.Name + "", "Falling");
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void Update(double delta)
    {
    }

    public override void Enter()
    {
        _targetVelocity = _Player.Velocity;
        _targetVelocity.Y = 0;
    }

    public override void Exit()
    {
    }
}
