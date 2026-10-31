using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
	private Button _resume;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Must keep running while the tree is paused, otherwise it couldn't unpause
		ProcessMode = ProcessModeEnum.Always;
		Hide();
		
		_resume = GetNode<Button>("CenterContainer/VBoxContainer/ResumeButton");
		_resume.Pressed += () => SetPaused(false);
		GetNode<Button>("CenterContainer/VBoxContainer/MainMenuButton").Pressed += OnMainMenuPressed;
		GetNode<Button>("CenterContainer/VBoxContainer/QuitButton").Pressed += () => GetTree().Quit();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (!@event.IsActionPressed("pause") || GetTree().CurrentScene is MainMenu)
		{
			return;
		}
		
		SetPaused(!GetTree().Paused);
		GetViewport().SetInputAsHandled();
	}

	private void SetPaused(bool paused)
	{
		GetTree().Paused = paused;
		Visible = paused;

		if (paused)
		{
			_resume.GrabFocus();
		}
	}

	private void OnMainMenuPressed()
	{
		// Unpause first, or the main menu would load frozen
		SetPaused(false);
		GetTree().ChangeSceneToFile("res://Menu/MainMenu.tscn");
	}
}
