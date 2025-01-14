using Godot;
using System;

public partial class Car : CharacterBody2D
{
	private Sprite2D _sprite_s; // Default (straight) sprite
	private Sprite2D _sprite_t; // Turning sprite
	
	// Car-specific sprites (custom cars)
	private Texture2D _sprite_italic = GD.Load<Texture2D>("res://assets/cars/italic/IT_S.png");
	private Texture2D _sprite_toros = GD.Load<Texture2D>("res://assets/cars/toros/00_toros.png");

	// Car selection reference
	//private CarSel _carSel;

	public override void _Ready()
	{
		Position = new Vector2(512, 600); // Set car starting position

		// Get nodes for sprites
		_sprite_s = GetNode<Sprite2D>("SpriteS");
		_sprite_t = GetNode<Sprite2D>("SpriteT");

		// Hide turning sprite initially
		_sprite_t.Visible = false;

		// Reference to the car selection node
		//_carSel = GetNode<CarSel>("res://ui/carSel/car_selection.tscn"); // Adjust the path as needed

		UpdateCarSprite();
		//GD.Print("Current Car = " + CarSel.sel);
	}

	public override void _PhysicsProcess(double delta)
	{
		HandleSpriteDirection();
	}

	public void HandleSpriteDirection()
	{
		if (Input.IsActionPressed("ui_left"))
		{
			_sprite_t.Visible = true;
			_sprite_s.Visible = false;
			_sprite_t.FlipH = false; // Facing left
		}
		else if (Input.IsActionPressed("ui_right"))
		{
			_sprite_t.Visible = true;
			_sprite_s.Visible = false;
			_sprite_t.FlipH = true; // Facing right
		}
		else
		{
			_sprite_t.Visible = false;
			_sprite_s.Visible = true;
			_sprite_s.FlipH = false; // Default orientation
		}
	}

	public void UpdateCarSprite()
	{
		// Switch sprite based on car selection
		/*if (CarSel.sel == 0) // Italic Car
		{
			_sprite_s.Texture = _sprite_italic;
			_sprite_t.Texture = _sprite_italic;
		}
		else if (CarSel.sel == 1) // Toros Car
		{
			_sprite_s.Texture = _sprite_toros;
			_sprite_t.Texture = _sprite_toros;
		}
		// Add more cars as needed
		*/
	}
}
