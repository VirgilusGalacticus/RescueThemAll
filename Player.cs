using Godot;
using System;

public class Player : KinematicBody
{
    [Export]
    public float Gravity { get; private set; }=10.0f;
    [Export]
    public float Acceleration { get; private set; }=1.5f;
    [Export]
    public float Speed { get; set; }=40.0f;
    [Export]
    PackedScene ropePart;
    [Export]
    private NodePath _helico;
    [Export]
    public NodePath _hingeJoint { get; set; }
    [Export]
    public float MaxHeight { get; set; }=20f;
    
    //TODO faire passer ropepart dans l'inventaire
    public int RopePartNumber { get; set; }
    private Vector3 _direction = new Vector3();
    private Vector3 _velocity = new Vector3();
    private HingeJoint _rootJoint;
    private Area _rescueArea;
    private AnimationPlayer _animationPlayer;
    private Character _characterToRescue;
    private Godot.Collections.Array<RopePart> _ropes = new Godot.Collections.Array<RopePart>();
    private RigidBody _grabBody;

    public override void _Ready()
    {
        base._Ready();
        _rootJoint = GetNode<HingeJoint>(_hingeJoint);
        _rescueArea = GetNode<Area>("RescueArea");
        _animationPlayer = GetNode<AnimationPlayer>(_helico+"/AnimationPlayer");
    }
    public override void _PhysicsProcess(float delta)
    {
        base._Process(delta);
        _direction = Vector3.Zero;
        if(Input.IsActionPressed("left")){
            RotationDegrees = new Vector3(0,0,0);
            _direction.x -= 1.0f;
        }
        else if(Input.IsActionPressed("right")){
            RotationDegrees = new Vector3(0,180,0);
            _direction.x += 1.0f;
        }
        else {
            _direction.x = 0.0f;
        }
        if(Input.IsActionPressed("down")){
            _direction.y -= 1.0f;
        }
        else if(Input.IsActionPressed("up") && GlobalTransform.origin.y < MaxHeight){
            _direction.y += 1.0f;
        }
        else {
            _direction.y = 0.0f;
        }

        if(Input.IsActionJustPressed("interact") && _characterToRescue !=null){
            _characterToRescue.QueueFree();
            _characterToRescue = null ;
            AddRopePart();
        }
        if(Input.IsActionJustReleased("detach")){
            DetachLastRopePart();
        }
        if(Input.IsActionJustPressed("grab") && _grabBody !=null && _ropes.Count>0){
            Grab(_grabBody, _ropes[_ropes.Count-1]);
        }
        else if(Input.IsActionJustReleased("grab") && _grabBody !=null && _ropes.Count>0){
            Ungrab(_ropes[_ropes.Count-1]);
        }
        _velocity = _velocity.LinearInterpolate(_direction*Speed,Acceleration * delta);
        _animationPlayer.Play("EngineOn");
        _animationPlayer.PlaybackSpeed =  _velocity.Length()/Speed*2.0f+1.0f;
        RotationDegrees = new Vector3(0,RotationDegrees.y,Mathf.Abs(_velocity.x));
        MoveAndSlide(_velocity, Vector3.Up);
    }

    private void AddRopePart(){
        
        var instance = ropePart.Instance<RopePart>();
        GetTree().Root.AddChild(instance);
        
        if(_ropes.Count == 0 ){
            instance.Translation = _rootJoint.GlobalTransform.origin - instance.GrabPosition.GlobalTransform.origin;
            _rootJoint.Nodes__nodeB = instance.GetPath();
            _rescueArea.Monitoring = false;
        } else {
            var lastRope = _ropes[_ropes.Count-1];
            instance.Translation = lastRope.NextJoin.GlobalTransform.origin - instance.GrabPosition.GlobalTransform.origin;
            lastRope.LinkRopePart(instance);
            
            lastRope.RescueArea.Disconnect("body_entered",this,nameof(OnRescueAreaEntered));
            lastRope.RescueArea.Disconnect("body_exited",this,nameof(OnRescueAreaExited));
        }
        _ropes.Add(instance);
        instance.RescueArea.Connect("body_entered",this,nameof(OnRescueAreaEntered));
        instance.RescueArea.Connect("body_exited",this,nameof(OnRescueAreaExited));
    }

    private void DetachLastRopePart(){
        if(_ropes.Count == 0)
            return;
        var lastRope = _ropes[_ropes.Count-1];
        
        lastRope.RescueArea.Disconnect("body_entered",this,nameof(OnRescueAreaEntered));
        lastRope.RescueArea.Disconnect("body_exited",this,nameof(OnRescueAreaExited));
        if(_ropes.Count == 1 ){
            _rescueArea.Monitoring = true;
            _rootJoint.Nodes__nodeB = string.Empty;
        }
        else {
            var preLastRope = _ropes[_ropes.Count-2];
            preLastRope.RescueArea.Connect("body_entered",this,nameof(OnRescueAreaEntered));
            preLastRope.RescueArea.Connect("body_exited",this,nameof(OnRescueAreaExited));
            preLastRope.BreakJoin();
        }
        _ropes.Remove(lastRope);

    }

    private void Grab(RigidBody body, RopePart rope){
        rope.LinkRopePart(body);     
    }
    private void Ungrab(RopePart rope){
        rope.BreakJoin();
    }

    public void OnRescueAreaEntered(Node body){
        GD.Print("Entered");
        if(body is Character && ((Character)body).Type == Character.CharacterType.ROPE){
            GD.Print("Character Entered");
            _characterToRescue = (Character)body;
        }else if(body is RigidBody && _ropes.Count >0){
            _grabBody = ((RigidBody)body);
        }
    }

    public void OnRescueAreaExited(Node body){
        if(body is Character && ((Character)body).Type == Character.CharacterType.ROPE){
            if( _characterToRescue ==  (Character)body){
                _characterToRescue = null;
            };
        }else if (body is RigidBody && ((RigidBody)body) == _grabBody && _ropes.Count >0){
            _grabBody = null;
        }
    }

}
