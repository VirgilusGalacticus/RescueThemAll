using Godot;
using System;

public class Door : StaticBody
{
    [Export]
    public NodePath LevierPath { get; private set; }
    private Tween _tween;
    private Vector3 _upPosition;
    private Vector3 _downPosition;
    public override void _Ready()
    {
        _tween = GetNode<Tween>("Tween");
        var openArea = GetNode(LevierPath).GetNode<Area>("OpenZone");
        openArea.Connect("body_entered",this,nameof(Open));
        openArea.Connect("body_exited",this,nameof(Close));
        _downPosition = Translation+Vector3.Down*3.0f;
        _upPosition = Translation;
    }

    public void Open(Node body){
        _tween.InterpolateProperty(this,"translation",Translation,_downPosition,2.0f);
        _tween.Start();
    }

    public void Close(Node body){
        _tween.InterpolateProperty(this,"translation",Translation,_upPosition,2.0f);
        _tween.Start();
    }
}
