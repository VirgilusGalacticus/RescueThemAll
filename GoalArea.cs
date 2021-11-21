using Godot;
using System;

public class GoalArea : Area
{
    [Export]
    public int GoalNumber { get; set; }=3;
    [Signal]
    public delegate void GoalEntered(int actual, int max);
    public override void _Ready()
    {
        
    }

    public void OnAreaEntered(Node body){
        if(body is Bloc){
            body.QueueFree();
            EmitSignal(nameof(GoalEntered));
        }

    }
}
