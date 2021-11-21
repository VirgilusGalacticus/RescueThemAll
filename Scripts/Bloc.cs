using Godot;
using System;

public class Bloc : RigidBody
{
    private Vector3 _startingPosition;
    public override void _Ready()
    {
        _startingPosition = GlobalTransform.origin;
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        if(GlobalTransform.origin.y < -1.0f){
            Translation = _startingPosition;
        }
    }
}
