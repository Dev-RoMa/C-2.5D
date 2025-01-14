using Godot;
using System;
using System.Collections.Generic;

public partial class Game : Node2D
{
	// Game Settings
	/*okay so the window sice width and height should be
	called from the game and not crated statically, im a
	dumb455 :3 */
	private static int width = 1024;       // Window width
	private static int height = 768;      // Window height
	private static int roadW = 2000;      // Road width
	private int segL = 200;               // Segment length
	private static float camD = 0.9f;     // Camera depth

	// Road Management
	private List<Line> lines = new List<Line>(); // List of road segments
	private int roadLines = 2000;                // Total road lines

	// Player Attributes
	private int pos = 0;                  // Player position along the road
	private int playerX = 0;              // Player lateral position
	private int startPos;                 // Starting position of the camera

	// Car Speed and Gear Logic
	private int[] gearing = { 0, 100, 150, 200, 400 }; // Gears and their speeds
	private int f = 0;                                 // Current gear index
	private float rpm = 0;                             // Current RPM
	public float maxRpm = 4000;                        // Maximum RPM
	private float minRpm = 800;                        // Minimum RPM

	// RPM Behavior
	private const float rpmDecayIdle = 1000f;          // RPM decay when idle
	private const float rpmDecay = 2000f;              // RPM decay when braking
	private const float accelerationFactor = 2.5f;     // RPM increase factor
	
	//Sprite for objects | tree
	private Sprite2D sTree;

	// Initialization
	public override void _Ready()
	{
		GenerateRoadLines();
	}

	// Main Loop
	public override void _Process(double delta)
	{
		//DebugPlayerStats();
		HandleGearChanges();
		UpdatePlayerMovement(delta);
		ResetRaceLoop();
		DrawRoad();
	}
	// Debugging Helper: Prints player stats
	/*private void DebugPlayerStats()
	{
		GD.Print($"StartPos = {startPos}");
		GD.Print($"Pos = {pos}");
		GD.Print($"Gear Index = {f}");
		GD.Print($"Speed = {gearing[f]}");
		GD.Print($"RPM = {rpm}");
	} */

	// Handle Gear Changes
	private void HandleGearChanges()
	{
		if (Input.IsActionJustPressed("shift"))
		{
			if (f < gearing.Length - 1)
			{
				f++;
				rpm = Mathf.Lerp(rpm, minRpm, 0.5f); // RPM drop when shifting up
			}
		}
		else if (Input.IsActionJustPressed("ctrl"))
		{
			if (f > 0)
			{
				f--;
				rpm = Mathf.Lerp(rpm, maxRpm * 0.7f, 0.3f); // RPM increase when shifting down
			}
		}
		f = Mathf.Clamp(f, 0, gearing.Length - 1); // Clamp gear index
	}

	// Update Player Movement
	private void UpdatePlayerMovement(double delta)
	{
		if (Input.IsActionPressed("ui_up"))
		{
			rpm += accelerationFactor * (gearing[f] * (float)delta); // Accelerate
		}
		else
		{
			rpm -= rpmDecayIdle * (float)delta; // Decay RPM when idle
		}

		if (Input.IsActionPressed("ui_down"))
		{
			rpm -= rpmDecay * (float)delta; // Brake
		}

		rpm = Mathf.Clamp(rpm, minRpm, maxRpm); // Clamp RPM
		pos += (int)(gearing[f] * (rpm / maxRpm)); // Update position

		if (Input.IsActionPressed("ui_right")) playerX += 20;
		if (Input.IsActionPressed("ui_left")) playerX -= 20;
	}

	// Reset the race loop when reaching the end
	private void ResetRaceLoop()
	{
		if (startPos >= roadLines - 300)
		{
			pos = 0;
		}
	}

	// Generate Road Lines
	private void GenerateRoadLines()
	{
		for (int i = 0; i < roadLines; i++)
		{
			Line line = new Line { z = i * segL };
			
			GD.Print($"Current Line Index: {i}");
			
			if (i > 300 && i < 700) 
			{
				line.curve = 0.5f; // Add curve to the road
			}
			/*
			if (i%20==0) {
				drawSprite();
			}
			*/
			lines.Add(line);
		}
	}


	// Draw the Road
	private void DrawRoad()
	{
		QueueRedraw();
	}
	public override void _Draw()
	{
		startPos = pos / segL;
		int camH = 1500 + 1 + (int)lines[startPos].y;

		float x = 0, dx = 0;
		int maxy = height;

		for (int n = startPos; n < startPos + 300; n++)
		{
			Line line = lines[n % lines.Count]; // Get the current line



			// Project the line's position for screen space
			line.Project(playerX - (int)x, camH, pos, width, height);
			x += dx;
			dx += line.curve;

			if (line.Y >= maxy) continue;

			maxy = (int)line.Y;

			// Draw the road segment using your method
			DrawRoadSegment(line, lines[(n - 1) % lines.Count]);
		}
	}
	
	// Draw Road Segments
	private void DrawRoadSegment(Line line, Line prevLine)
	{
		// Grass
		Color grass = (line.z / 3) % 2 == 0 ? new Color(0, 154 / 255f, 0) : new Color(16 / 255f, 200 / 255f, 16 / 255f);
		DrawColoredPolygon(new Vector2[]
		{
			new Vector2(0, prevLine.Y),
			new Vector2(width, prevLine.Y),
			new Vector2(width, line.Y),
			new Vector2(0, line.Y)
		}, grass);

		// Rumble Strips
		Color rumble = (line.z / 3) % 2 == 0 ? new Color(0, 0, 0) : new Color(1, 1, 1);
		DrawColoredPolygon(new Vector2[]
		{
			new Vector2(prevLine.X - prevLine.W * 1.2f, prevLine.Y),
			new Vector2(line.X - line.W * 1.2f, line.Y),
			new Vector2(line.X + line.W * 1.2f, line.Y),
			new Vector2(prevLine.X + prevLine.W * 1.2f, prevLine.Y)
		}, rumble);
		// Road
		Color road = new Color(105 / 255f, 105 / 255f, 105 / 255f);
		DrawColoredPolygon(new Vector2[]
		{
			new Vector2(prevLine.X - prevLine.W, prevLine.Y),
			new Vector2(line.X - line.W, line.Y),
			new Vector2(line.X + line.W, line.Y),
			new Vector2(prevLine.X + prevLine.W, prevLine.Y)
		}, road);
	}
	// Line Class: Represents each segment of the road
	public class Line
	{
		// World position
		public float x, y, z; 
		
		// Screen position
		public float X, Y, W; 
		public float scale, curve, spriteX;

		// Project method for calculating screen position
		public void Project(int camX, int camY, int camZ, int width, int height)
		{
			float dz = z - camZ;
			dz = Mathf.Max(dz, 0.1f); // Prevent division by zero

			scale = Game.camD / dz;
			X = (width / 2) + scale * (x - camX) * (width / 2);
			Y = (height / 2) - scale * (y - camY) * (height / 2);
			W = scale * Game.roadW * (width / 2);
		}
	}
public void drawSprite()
// Load the scene resource and instance it
	{
		PackedScene treeScene = (PackedScene)ResourceLoader.Load("res://assets/enviroment/trees/sprite_2d.tscn");
		
		if (treeScene != null)
		{
			sTree = (Sprite2D)treeScene.Instantiate();
			AddChild(sTree); // Add the Sprite2D instance to the current node as a child
			sTree.Visible = true;
		}
		else
		{
			GD.PrintErr("Failed to load tree scene.");
		}
	}
} //END OF CODE
/*
	
	public void DrawSprite(Sprite sprite, float spriteX, float scale, float W, float clip, float X, float Y)
 {
	//SET THE SPRITE
	sTree = GD.Load<Texture>("res://assets/environment/trees/tree.png");
	
	Sprite sprite = sTree;
	// Get sprite dimensions
	int w = sprite.Texture.GetWidth();
	int h = sprite.Texture.GetHeight();

	// Calculate destination values
	float destX = X + scale * spriteX * width / 2;
	float destY = Y + 4;
	float destW = w * W / 266;
	float destH = h * W / 266;

	destX += destW * spriteX; // Offset X
	destY += 0;              // Offset Y (adjust if needed)

	// Clip height
	float clipH = destY + destH - clip;
	if (clipH < 0) clipH = 0;

	if (clipH >= destH) return;

	// Update the texture rect to clip the sprite
	Rect2 textureRect = sprite.RegionRect;
	textureRect.Size = new Vector2(w, h - h * clipH / destH);
	sprite.RegionRect = textureRect;

	// Update scale and position
	sprite.Scale = new Vector2(destW / w, destH / h);
	sprite.Position = new Vector2(destX, destY);

	// Add sprite to the scene if not already added
	if (!sprite.IsInsideTree())
	{
		AddChild(sprite);
	}
 }
}
*/
	
	/*public void drawSprite()
	{
		Sprite s = sprite;
		int w = s.getTextureRect().width;
		int h = s.getTextureRect().height;
		
		float destX = X + scale * spriteX * width/2;
		float destY = Y + 4;
		float destW = w * W / 266;
		float destH = h * W / 266;
		
		destX += destW * spriteX; //offsetX
		destY += * (-1) //offsetY
		
		float clipH = destY+destH-clip;
		if (clipH<0) clipH=0;
		
		if (clipH>=destH) return;
		s.setTextureRect(IntRect(0,0,w,h-h*clipH/destH));
		s.setScale(destW/w,destH/h);
		s.setPosition(destX, destY);
		app.draw(s);
	}
} */
/*
	// Scene for the object (tree)
	//public PackedScene ObjectScene = GD.Load<PackedScene>("res://assets/enviroment/trees/obj.tscn");
	//private Node2D spawnedObject;

	// Spawn the object at a specific segment of the road
	private void SpawnObject(int roadSegment)
	{
		Line segment = lines[roadSegment % lines.Count];
		spawnedObject = (Node2D)ObjectScene.Instantiate();
		spawnedObject.Position = new Vector2(segment.X, segment.Y);
		AddChild(spawnedObject);
	}

	private void CheckCollision()
	{
		if (spawnedObject == null) return;

		Line playerSegment = lines[pos / segL % lines.Count];
		Line objectSegment = lines[(int)(spawnedObject.Position.Y / segL) % lines.Count];

		// Simple collision detection
		if (Mathf.Abs(playerSegment.z - objectSegment.z) < segL * 0.5f) // Adjust threshold as needed
		{
			GD.Print("Collision Detected!");
			// Handle collision (e.g., remove object, decrease speed)
			spawnedObject.QueueFree();
			spawnedObject = null;
		}
	}
	
		private void GenerateRoadLines()
	{
		for (i = 0; i < roadLines; i++)
		{
			Line line = new Line
			{
				z = i * segL
			};

			if (i > 300 && i < 700) line.curve = 0.5f;

			// Add tree sprite every 20 segments
			if (i % 20 == 0)
			{
				line.spriteX = -2.5f;
				//line.sprite = ObjectScene; // Assigning the tree sprite scene to the line
			}

			lines.Add(line);
		}
	}
*/

	/*if (spawnedObject != null && spawnedObject.Position.Y > height)
	{
	spawnedObject.QueueFree();
	//spawnedObject = null;
	GD.Print("Tree removed from the scene");
	}*/

/*
			// Check and spawn the tree sprite if not already present
			if (line.sprite != null && spawnedObject == null) // Ensures only one tree per line
			{
				SpawnObject(n);
			}
*/

/*
	// Check for Collisions
	private void CheckCollision()
	{
		if (spawnedObject == null) return;

		Line playerSegment = lines[pos / segL % lines.Count];
		Line objectSegment = lines[(int)(spawnedObject.Position.Y / segL) % lines.Count];

		if (Mathf.Abs(playerSegment.z - objectSegment.z) < segL * 0.5f)
		{
			GD.Print("Collision Detected!");
			spawnedObject.QueueFree();
			spawnedObject = null;
		}
	}
*/

/*
	// Spawn Object at a Specific Segment
	private void SpawnObject(int roadSegment)
	{
		Line segment = lines[roadSegment % lines.Count];
		spawnedObject = (Node2D)ObjectScene.Instantiate();
		
		// Use segment.spriteX for horizontal positioning
		spawnedObject.Position = new Vector2(segment.X + segment.spriteX, segment.Y);
		AddChild(spawnedObject);

		GD.Print($"Tree spawned at: {spawnedObject.Position}");
	}
*/

/*
			if (i % 2 == 0)
			{
				line.spriteX = roadW * 0.75f; // Position tree off the road
				line.sprite = ObjectScene;   // Assign tree sprite
			}
*/

/*
		public PackedScene sprite; // Sprite for roadside objects
		public float spriteX = 0;  // Sprite offset
*/

	/*/ Object Spawning
	public PackedScene ObjectScene = GD.Load<PackedScene>("res://assets/enviroment/trees/obj.tscn");
	private Node2D spawnedObject; */

		// Check if the line has a sprite assigned and if it should be drawn
		/*if (line.sprite != null)
		{
			// Create a Sprite2D node for the tree and draw it
			Sprite2D treeSprite = new Sprite2D(); // Use Sprite2D instead of Sprite
			DrawSprite(treeSprite, line.spriteX, 1.0f, line.W, 0.0f, line.X, line.Y); // Adjust parameters as needed
		} */
