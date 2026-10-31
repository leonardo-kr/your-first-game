using Godot;
using System;
using YourFirstGame.Player;

public partial class HeadPlayer : BasePlayer
{
	public override void _Ready()
	{
		Sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		Collision = GetNode<CollisionShape2D>("CollisionShape2D");
	}
	
	public override void _PhysicsProcess(double delta)
	{
		ApplyGravity(delta);
		HandleMovement(delta);

		MoveAndSlide();
	}

	private void UpdateTimers(double delta)
	{
		InvincibilityTimer = Mathf.Max(InvincibilityTimer - (float) delta, 0.0f);
	}
	
	private void ApplyGravity(double delta)
	{
		if (IsOnFloor())
		{
			return;
		}
  
		float gravityScale = GravityScale;
  
		if (Velocity.Y < 0)
		{
			gravityScale *= FallGravityScale;
		}
		
		Velocity += GetGravity() * gravityScale * (float) delta;
		Velocity = new Vector2(Velocity.X, Mathf.Min(Velocity.Y, MaxFallSpeed));
	}

	private void HandleMovement(double delta)
	{
		Vector2 direction = Input.GetVector("move_left", "move_right", "look_up", "look_down");
		float movementDirection = Input.GetAxis("move_left", "move_right");

		if (direction != Vector2.Zero)
		{
		
			if (Math.Abs(direction.Y) >= Math.Abs(direction.X))
			{
				FacingDirection = direction.Y > 0 ? Vector2.Down : Vector2.Up;
			} else
			{
				FacingDirection = direction.X > 0 ? Vector2.Right : Vector2.Left;
			}
		}

		// only needed for controller, but doesnt impact keyboard
		// always move at the same speed, by setting the movement director to 1f 
		if (movementDirection > 0)
		{
			Sprite.FlipH = false;
			movementDirection = 1f;
		} else if (movementDirection < 0) {
			Sprite.FlipH = true;
			movementDirection = -1f;
		} else
		{
			movementDirection = 0f;
		}
		
		if (KnockBackTimer > 0f)
		{
			KnockBackTimer -= (float)delta;
		} else
		{	
			Velocity = new Vector2(movementDirection * Speed, Velocity.Y);
		}
	}
}
