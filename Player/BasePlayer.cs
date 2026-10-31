using Godot;

namespace YourFirstGame.Player;

public abstract partial class BasePlayer : CharacterBody2D
{
	[Export] public int Health = 10;
	[Export] public float Speed = 275.0f;
	
	// Gravity stuff
	[Export] public float GravityScale = 1.5f;
	[Export] public float FallGravityScale = 1.1f;
	[Export] public float MaxFallSpeed = 1100.0f;
}
