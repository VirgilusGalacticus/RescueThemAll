using Godot;
using System;

public class UI : Control
{
    AnimatedSprite _grabE;
    Control _grabControl;
    private int _actual;
    public override void _Ready()
    {
        _grabE = GetNode<AnimatedSprite>("GrabE");
        _grabE.Visible = false;
        _grabControl = GetNode<Control>("GrabControls");
        _grabControl.Visible = false;
    }

    public void DisplayGrabE(bool display){
        _grabE.Visible = display;
    }

    public void DisplayControl(bool display){
        _grabControl.Visible = display;
    }

    public void OnTimerTimeout(){
        GetNode<Label>("WellDone").Visible = false;
    }

    public void _on_GoalArea_body_entered(Node body){
        if(body is Bloc){
            body.QueueFree();
            GetNode<Label>("WellDone").Visible = true;
            GetNode<Timer>("Timer").Start();
            _actual++;
            GetNode<Label>("Label").Text = $"{_actual}/3";
            if(_actual == 3 ){
                GetNode<Control>("EndGame").Visible = true;
            }
        }
    }
}
