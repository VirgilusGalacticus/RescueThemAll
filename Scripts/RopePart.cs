using Godot;
using System;

public class RopePart : RigidBody
{
    [Export]
    public PackedScene CharacterPrefab { get; private set; }
    public bool IsLinked { get; set; }    
    RopePart linkedRope;
    public HingeJoint NextJoin { get; private set; }
    public Position3D GrabPosition { get; private set; }
    public Area RescueArea { get; private set; }
    private bool _hurtSomething;

    public override void _EnterTree()
    {
        NextJoin = GetNode<HingeJoint>("HingeJoint");
        GrabPosition = GetNode<Position3D>("GrabPosition");
        RescueArea = GetNode<Area>("RescueArea");
    }
    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        if(_hurtSomething && NextJoin.Nodes__nodeB == string.Empty && LinearVelocity.Length() <2.0f){
            var instance = CharacterPrefab.Instance<Character>();
            GetTree().Root.AddChild(instance);
            instance.Translation= GlobalTransform.origin;
            QueueFree();
        }
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

    public void OnRopePartCollide(Node body){
        if(!IsLinked && !(body is Player)){
            _hurtSomething = true;
        }
    }

}
