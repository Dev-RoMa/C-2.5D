using Godot;
using System;

public partial class CarSel : Control
{
	public int sel = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//GD.Print("sel = " +sel);
	}
	public void _on_button_prev_pressed(){
		sel -= 1;
		GD.Print("Prev!");
		GD.Print("sel = " +sel);
	}
	public void _on_button_next_pressed(){
		sel += 1;
		GD.Print("Next!");
		GD.Print("sel = " +sel);
		
	}
	public void _on_button_select_pressed(){
		//CHANGE SCENE
		GD.Print("select!");
	}
	public void _on_button_back_pressed(){
		//GO_BACKTO MAIN MENU
		GD.Print("back!");
	}
}
