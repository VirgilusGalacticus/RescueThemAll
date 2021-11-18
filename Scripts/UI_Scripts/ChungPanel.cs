using Godot;
using System;

public class ChungPanel : Panel
{
    [Export]
    public NodePath PlayerPath { get; private set; }
    private Player _player;
    public override void _Ready()
    {
        _player = GetNode<Player>(PlayerPath);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        GetNode<Label>("Label").Text = _player.RopePartNumber.ToString();
    }
}
