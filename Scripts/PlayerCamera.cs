using Godot;
using System;

public class PlayerCamera : Camera
{
    [Export]
    NodePath targetPath;
    [Export]
    float followingSpeed=20.0f;
    private Spatial _target;
    public override void _Ready()
    {
        _target = GetNode<Spatial>(targetPath);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        float zFactor = 25f+ 40f *Mathf.Clamp(GlobalTransform.origin.y/5.0f-1.0f,0f,1f) ;
        Vector3 targetPosition = new Vector3 (_target.GlobalTransform.origin.x ,_target.GlobalTransform.origin.y, zFactor) ;

        Translation = Translation.LinearInterpolate(targetPosition, delta*followingSpeed);
        
    }
}
