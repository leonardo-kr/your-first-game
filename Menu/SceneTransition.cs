using Godot;
using System;
using System.Threading.Tasks;

public partial class SceneTransition : CanvasLayer
{
	[Export] public float FadeDuration = 0.3f;

	private ColorRect _fade;
	private bool _busy;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Must also run when called from the paused game
		ProcessMode = ProcessModeEnum.Always;

		_fade = GetNode<ColorRect>("Fade");
		_fade.Modulate = new Color(1, 1, 1, 0);
		_fade.MouseFilter = Control.MouseFilterEnum.Ignore;
	}

	public async void ChangeScene(string path)
	{
		if (_busy)
		{
			return;
		}
		
		_busy = true;
		_fade.MouseFilter = Control.MouseFilterEnum.Stop;
		await Fade(1f);
		GetTree().ChangeSceneToFile(path);
		await ToSignal(GetTree(), SceneTree.SignalName.SceneChanged);
		await ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);
		await Fade(0f);
		
		_fade.MouseFilter = Control.MouseFilterEnum.Ignore;
		_busy = false;
	}

	private async Task Fade(float alpha)
	{
		var tween = CreateTween();
		tween.TweenProperty(_fade, "modulate:a", alpha, FadeDuration);
		await ToSignal(tween, Tween.SignalName.Finished);
	}
}
