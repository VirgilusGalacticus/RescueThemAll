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
    //TODO faire passer ropepart dans l'inventaire
    public int RopePartNumber { get; set; }
    private Vector3 _direction = new Vector3();
    private Vector3 _velocity = new Vector3();
    private HingeJoint _rootJoint;
    private RopePart _lastAddedRope;
    private RayCast _landingRay1;
    private RayCast _landingRay2;
    private Area _rescueArea;
    private AnimationPlayer _animationPlayer;
    private Position3D _rescuePosition;

    public override void _Ready()
    {
        base._Ready();
        _rootJoint = GetNode<HingeJoint>(_hingeJoint);
        _landingRay1 = GetNode<RayCast>("LandingRay1");
        _landingRay2 = GetNode<RayCast>("LandingRay2");
        _rescueArea = GetNode<Area>("RescueArea");
        _animationPlayer = GetNode<AnimationPlayer>(_helico+"/AnimationPlayer");
        _rescuePosition = GetNode<Position3D>("RescuePosition");
        _rescueArea.Monitoring = false;
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
        else if(Input.IsActionPressed("up")){
            _direction.y += 1.0f;
        }
        else {
            _direction.y = 0.0f;
        }

        if(Input.IsActionJustPressed("ui_down") && RopePartNumber >0){
            AddRopePart();
        }
        if(Input.IsActionJustPressed("interact") && _landingRay1.IsColliding() && _landingRay2.IsColliding()){
            _rescueArea.Monitoring = true;
            DelayedAreaDisabling(_rescueArea);
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
        Vector3 position = _rootJoint.GlobalTransform.origin;
        position.y -= instance.GrabPosition.Translation.y;
        instance.Translation = position;
        _rootJoint.Nodes__nodeB = instance.GetPath();
        if(_lastAddedRope!=null){
            _lastAddedRope.Translate(Vector3.Down*(_lastAddedRope.GrabPosition.GlobalTransform.origin.DistanceTo(instance.NextJoin.GlobalTransform.origin)));
            instance.LinkRopePart(_lastAddedRope);
        }
        _lastAddedRope = instance;
    }
    
    public void OnRescueAreaEntered(Node body){
        GD.Print("Entered");
        if(body is Character){
            GD.Print("Character Entered");
            var character = (Character)body;
            if(!character.IsConnected(nameof(Character.MovementEnd),this, nameof(OnCharacterMouvementEnd)))
                character.Connect(nameof(Character.MovementEnd),this, nameof(OnCharacterMouvementEnd));
            ((Character)body).Rescue(_rescuePosition);
        }
    }

    public void OnCharacterMouvementEnd(Character character){
        GD.Print("Mouvement end for "+character.Name);
        character.Disconnect(nameof(Character.MovementEnd),this, nameof(OnCharacterMouvementEnd));
        if(_rescuePosition.GlobalTransform.origin.DistanceTo(new Vector3(GlobalTransform.origin.x, GlobalTransform.origin.y, character.GlobalTransform.origin.z))< 3.0f){
            if(character.Type == Character.CharacterType.ROPE)
                RopePartNumber ++;
            character.QueueFree();
            AddRopePart();
        }
    }

    private async void DelayedAreaDisabling(Area area){
        await ToSignal(GetTree().CreateTimer(1.0f),"timeout");
        area.Monitoring = false;
    }

}
