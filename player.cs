using Godot;
using System;
using System.Collections;

public partial class player : CharacterBody3D
{
    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();
    }
}
