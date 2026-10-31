using Godot;
using System;

public partial class MainMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var play = GetNode<Button>("CenterContainer/VBoxContainer/PlayButton");
		play.Pressed += OnPlayPressed;
		
		var quit = GetNode<Button>("CenterContainer/VBoxContainer/QuitButton");
		quit.Pressed += OnQuitPressed;

		play.GrabFocus(); // lets controller/keyboard users navigate without a mouse
	}

	private void OnPlayPressed()
	{
		GetTree().ChangeSceneToFile("res://Level/Level1.tscn");
	}
	
	private void OnQuitPressed()
	{
		GetTree().Quit();
	}
}
