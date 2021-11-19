using Godot;
using System;

public class RopePart : RigidBody
{
    RopePart linkedRope;
    public HingeJoint NextJoin { get; private set; }
    public Position3D GrabPosition { get; private set; }
    public Area RescueArea { get; private set; }
    public override void _EnterTree()
    {
        NextJoin = GetNode<HingeJoint>("HingeJoint");
        GrabPosition = GetNode<Position3D>("GrabPosition");
        RescueArea = GetNode<Area>("RescueArea");
    }

    public void LinkRopePart(RopePart nextRope){
        linkedRope = nextRope;
        NextJoin.Nodes__nodeB = linkedRope.GetPath();
        NextJoin.Nodes__nodeA = GetPath();

    }
    public void LinkRopePart(RigidBody body){
        NextJoin.Nodes__nodeB = body.GetPath();
        NextJoin.Nodes__nodeA = GetPath();
    }
    public void BreakJoin(){
        NextJoin.Nodes__nodeB = string.Empty;
        NextJoin.Nodes__nodeA = string.Empty;
        linkedRope = null;
    }

}
