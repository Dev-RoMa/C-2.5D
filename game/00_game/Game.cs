using Godot;
using System;
using System.Collections.Generic;

public partial class Game : Node2D
{
	// Window size
	private int width = 1024;
	private int height = 768;
	private static int roadW = 2000;
	private int segL = 200;
	private static float camD = 0.9f;

	// List of road lines
	private List<Line> lines = new List<Line>();

	// Player position
	private int pos = 0;
	private int playerX = 0;

	// Background texture
	//private Sprite2D background;
	
	public override void _Ready()
	{
		//// Load background texture (make sure to add bg.png to your project)
		//background = new Sprite2D();
		//Texture bg = (Texture)GD.Load("res://bg.png");
		//background.Texture = bg;
		//background.SetRegionEnabled(true);
		//background.RegionRect = new Rect2(0, 0, width, height);
		//AddChild(background);

		// Generate road lines
		GenerateRoadLines();

		// Set up camera position or viewport logic here if necessary
	}

	public override void _Process(double delta)
	{
		// Handle player controls
		if (Input.IsActionPressed("ui_up")) pos += 20000;
		if (Input.IsActionPressed("ui_down")) pos -= 200;
		if (Input.IsActionPressed("ui_right")) playerX += 200;
		if (Input.IsActionPressed("ui_left")) playerX -= 200;

		// Draw the road
		DrawRoad();
	}


	private void GenerateRoadLines()
	{
		// Generate road lines with curves and hills
		for (int i = 0; i < 1600; i++)
		{
			Line line = new Line
			{
				z = i * segL
			};

			if (i > 300 && i < 700) line.curve = 0.5f;
			if (i > 750) line.y = (float)Math.Sin(i / 30.0) * 1500;

			lines.Add(line);
		}
	}

	private void DrawRoad()
	{
		int startPos = pos / segL;
		int camH = 1500 + 1 + (int)lines[startPos].y;

		float x = 0, dx = 0;
		int maxy = height;

		// Loop through road lines and draw quads
		for (int n = startPos; n < startPos + 300; n++)
		{
			Line line = lines[n % lines.Count];
			line.Project(playerX - (int)x, camH, pos);

			// Update curve
			x += dx;
			dx += line.curve;

			// Skip lines that go off-screen
			if (line.Y >= maxy) continue;
			maxy = (int)line.Y;

			// Colors for road, rumble, and grass
			Color grass = (n / 3) % 2 == 0 ? new Color(0, 154, 0) : new Color(16 / 255f, 200 / 255f, 16 / 255f);
			Color rumble = (n / 3) % 2 == 0 ? new Color(0, 0, 0) : new Color(1, 1, 1);
			Color road = (n / 3) % 2 == 0 ? new Color(105 / 255f, 105 / 255f, 105 / 255f) : new Color(107 / 255f, 107 / 255f, 107 / 255f);

			Line prevLine = lines[(n - 1) % lines.Count];

			// Draw quads for different parts of the road
			DrawQuad(grass, 0, (int)prevLine.Y, width, 0, (int)line.Y, width);
			DrawQuad(rumble, (int)prevLine.X, (int)prevLine.Y, (int)(prevLine.W * 1.2f), (int)line.X, (int)line.Y, (int)(line.W * 1.2f));
			DrawQuad(road, (int)prevLine.X, (int)prevLine.Y, (int)prevLine.W, (int)line.X, (int)line.Y, (int)line.W);
		}
	}

	private void DrawQuad(Color color, int x1, int y1, int w1, int x2, int y2, int w2)
	{
		// Create a quad shape and draw it
		var polygon = new Polygon2D();
		polygon.Polygon = new Vector2[]
		{
			new Vector2(x1 - w1, y1),
			new Vector2(x2 - w2, y2),
			new Vector2(x2 + w2, y2),
			new Vector2(x1 + w1, y1)
		};
		polygon.Color = color;
		AddChild(polygon);
	}

	// Line class that handles road data and projection
	public class Line
	{
		public float x, y, z;
		public float X, Y, W;
		public float scale, curve;

		public Line()
		{
			x = y = z = 0;
		}

		// Projection from world to screen
		public void Project(int camX, int camY, int camZ)
		{
			scale = camD / (z - camZ);
			X = (1 + scale * (x - camX)) * 1024 / 2;
			Y = (1 - scale * (y - camY)) * 768 / 2;
			W = scale * roadW * 1024 / 2;
		}
	}
}
