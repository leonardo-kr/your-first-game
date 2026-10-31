using Godot;
using System;
using YourFirstGame.Player;

public partial class HeadPlayer : BasePlayer
{
	[Export] public float JumpVelocity = -800.0f;
	[Export] public float JumpCutMultiplier = 0.4f;
	[Export] public float CoyoteTime = 0.1f;
	[Export] public float JumpBufferTime = 0.1f;

	private float _invincibilityTimer = 0f;
	private float _invincibilityDuration = 1.5f;

	private float _knockBackTimer = 0f;
	private float _knockBackDuration = 0.2f;
	private float _receivedKnockBackStrengthX = 300;
	private float _receivedKnockBackStrengthY = 500;
	
	public Area2D Hurtbop;

	private Vector2 _facingDirection = Vector2.Right;
	
	private Sprite2D _sprite;
	
	private float _coyoteTimer = 0.0f;
	private float _jumpBufferTimer = 0.0f;

	public override void _Ready()
	{
		// Hurtbop = GetNode<Area2D>("Hurtbop");
		_sprite = GetNode<Sprite2D>("Sprite2D");
	}
	
	public override void _PhysicsProcess(double delta)
	{
		UpdateTimers(delta);
		ApplyGravity(delta);
		// HandleJump();
		HandleMovement(delta);

		MoveAndSlide();
	}

	private void UpdateTimers(double delta)
	{
		if (IsOnFloor())
		{
			_coyoteTimer = CoyoteTime;
		}
		else
		{
			_coyoteTimer = Mathf.Max(_coyoteTimer - (float) delta, 0.0f);
		}
		
		_jumpBufferTimer = Mathf.Max(_jumpBufferTimer - (float) delta, 0.0f);
		_invincibilityTimer = Mathf.Max(_invincibilityTimer - (float) delta, 0.0f);
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
		
		Velocity += GetGravity() * gravityScale * (float)delta;
		Velocity = new Vector2(Velocity.X, Mathf.Min(Velocity.Y, MaxFallSpeed));
	}

	private void Jump()
	{
		Velocity = new Vector2(Velocity.X, JumpVelocity);
	}

	private void HandleMovement(double delta)
	{
		Vector2 direction = Input.GetVector("move_left", "move_right", "look_up", "look_down");
		float movementDirection = Input.GetAxis("move_left", "move_right");
		GD.Print(_sprite.FlipH);
		// _sprite.RotationDegrees = movementDirection < 0 ? 90 : -90;

		if (direction != Vector2.Zero)
		{
		
			if (Math.Abs(direction.Y) >= Math.Abs(direction.X))
			{
				_facingDirection = direction.Y > 0 ? Vector2.Down : Vector2.Up;
			} else
			{
				_facingDirection = direction.X > 0 ? Vector2.Right : Vector2.Left;
			}
		}

		// only needed for controller, but doesnt impact keyboard
		// always move at the same speed, by setting the movement director to 1f 
		if (movementDirection > 0)
		{
			_sprite.FlipH = false;
			movementDirection = 1f;
		} else if (movementDirection < 0) {
			_sprite.FlipH = true;
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
