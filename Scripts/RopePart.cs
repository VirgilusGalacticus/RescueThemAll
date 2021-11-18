using Godot;
using System;

public class RopePart : RigidBody
{
    [Export]
    public float RuptureMagnitude { get; set; }=20.0f;
    [Signal]
    public delegate void Broken();
    RopePart linkedRope;
    public HingeJoint NextJoin { get; private set; }
    public Position3D GrabPosition { get; private set; }
    private float _lastVelocity;
    public override void _EnterTree()
    {
        NextJoin = GetNode<HingeJoint>("HingeJoint");
        GrabPosition = GetNode<Position3D>("GrabPosition");
    }

    public void LinkRopePart(RopePart nextRope){
        linkedRope = nextRope;
        linkedRope.Connect(nameof(Broken),this,nameof(OnBroken));
        NextJoin.Nodes__nodeB = linkedRope.GetPath();
        NextJoin.Nodes__nodeA = GetPath();

    }
    private void DisconnectNextRope(){
        NextJoin.Nodes__nodeB = string.Empty;
        NextJoin.Nodes__nodeA = string.Empty;
        linkedRope.Disconnect(nameof(Broken),this,nameof(OnBroken));
        linkedRope = null;
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        _lastVelocity = LinearVelocity.Length();
    }

    public void OnBroken(){
        DisconnectNextRope();
    }
}
