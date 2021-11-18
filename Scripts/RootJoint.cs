using Godot;
using System;

public class RootJoint : HingeJoint
{
    [Export]
    public NodePath _positionNode { get; private set; }
    private Position3D _targetPosition;
    public override void _Ready()
    {
        _targetPosition = GetNode<Position3D>(_positionNode);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess (float delta)
    {
        GlobalTransform = new Transform(Basis.Identity, _targetPosition.GlobalTransform.origin);
    }
}
