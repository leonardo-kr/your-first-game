using Godot;
using System;
using YourFirstGame.Player;

public partial class HeadPlayer : BasePlayer
{
	private float _invincibilityTimer = 0f;
	private float _invincibilityDuration = 1.5f;

	private float _knockBackTimer = 0f;
	private float _knockBackDuration = 0.2f;
	private float _receivedKnockBackStrengthX = 300;
	private float _receivedKnockBackStrengthY = 500;

	public override void _Ready()
	{
		// Hurtbop = GetNode<Area2D>("Hurtbop");
		Sprite = GetNode<Sprite2D>("Sprite2D");
        Collision = GetNode<CollisionShape2D>("CollisionShape2D");
	}
	
	public override void _PhysicsProcess(double delta)
	{
		ApplyGravity();
		HandleMovement(delta);

		MoveAndSlide();
    }

	private void UpdateTimers(double delta)
	{
		_invincibilityTimer = Mathf.Max(_invincibilityTimer - (float) delta, 0.0f);
	}
	
	private void ApplyGravity()
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
		
		Velocity += GetGravity() * gravityScale;
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
		
		if (_knockBackTimer > 0f)
		{
			_knockBackTimer -= (float)delta;
		} else
		{	
			Velocity = new Vector2(movementDirection * Speed, Velocity.Y);
		}
	}
}
