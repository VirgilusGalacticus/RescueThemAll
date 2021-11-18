using Godot;
using System;

public class Character : KinematicBody
{
    [Export]
    public CharacterType Type { get; private set; }
    [Export]
    public float Speed { get; set; }=3.0f;
    [Export]
    private NodePath _model;
    [Signal]
    public delegate void MovementEnd(Character character);
    public Position3D TargetPosition { get; set; }
    bool emitMoveEndSignal;
    private Timer _timer;
    private AnimationPlayer _animationPlayer;
    private static string idleAnimation = "Idle";
    private static string walkAnimation = "Walking";
    public override void _Ready()
    {
        _timer= GetNode<Timer>("WaitTimer");
        var rng= new RandomNumberGenerator();
        _timer.WaitTime = rng.RandfRange(1.0f,3.0f);
        _timer.Start();
        rng.Dispose();
        _animationPlayer = GetNode<AnimationPlayer>(_model+"/AnimationPlayer");
        _animationPlayer.CurrentAnimation = idleAnimation;
        _animationPlayer.Play();
    }

    public override void _PhysicsProcess(float delta)
    {
        if(TargetPosition!=null){
            var distance = GlobalTransform.origin.DistanceTo(TargetPosition.GlobalTransform.origin);
            if(distance < 0.1f){
                if(emitMoveEndSignal){
                    EmitSignal(nameof(MovementEnd),this);
                }
                return;
            }
            var direction = GlobalTransform.origin.DirectionTo(TargetPosition.GlobalTransform.origin).Normalized();
            MoveAndSlide(direction*Speed * delta);
        }
    }
    
    public void Rescue(Position3D target){
        TargetPosition = target;
        emitMoveEndSignal = true;
        _animationPlayer.CurrentAnimation = walkAnimation;
        _animationPlayer.GetAnimation(_animationPlayer.CurrentAnimation).Loop = true;
    }
    public void OnWaitTimerTimeout(){
        //TODO : mettre en place le choix aléatoire de target
    }

    public enum CharacterType {
        NORMAL,
        ROPE
    }
}
