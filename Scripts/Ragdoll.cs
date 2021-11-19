using Godot;
using System;

public class Ragdoll : Spatial
{
    [Export]
    public NodePath SkeletonPath { get; private set; }
    private Skeleton _skeleton;
    public override void _Ready()
    {
        _skeleton = GetNode<Skeleton>(SkeletonPath);

        _skeleton.PhysicalBonesStartSimulation();
    }
}
