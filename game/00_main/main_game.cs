using Godot;
using System;

public partial class main_game : Node
{
	// Called when the node enters the scene tree for the first time.
	private Game game;
	public override void _Ready()
	{
		game = new Game();
		AddChild(game);
		//game.maxRpm = 2000;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		game.maxRpm = 2000;
	}
}
