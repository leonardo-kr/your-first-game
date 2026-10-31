using Godot;
using System;
using System.Threading.Tasks;

public partial class SceneTransition : CanvasLayer
{
	[Export] public float FadeDuration = 0.3f;

	private ColorRect _fade;
	private bool _busy;

	private const string LoadTimesFile = "user://load_times.cfg";
	private const string LoadTimesSection = "load_times";
	private const float ShowProgressAfter = 0.15f; // seconds; avoids a flashing bar on instant loads

	private ProgressBar _progressBar;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Must also run when called from the paused game
		ProcessMode = ProcessModeEnum.Always;

		_fade = GetNode<ColorRect>("Fade");
		_fade.Modulate = new Color(1, 1, 1, 0);
		_fade.MouseFilter = Control.MouseFilterEnum.Ignore;
		
		_progressBar = GetNode<ProgressBar>("Fade/ProgressBar");
		_progressBar.Hide();
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

		var scene = await LoadWithProgress(path);
		if (scene != null)
		{
			GetTree().ChangeSceneToPacked(scene);
			await ToSignal(GetTree(), SceneTree.SignalName.SceneChanged);
			// Let the expensive first frame render behind the black screen
			await ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);
		}
		
		await Fade(0f);
		_progressBar.Hide();

		_fade.MouseFilter = Control.MouseFilterEnum.Ignore;
		_busy = false;
	}

	private async Task Fade(float alpha)
	{
		var tween = CreateTween();
		tween.TweenProperty(_fade, "modulate:a", alpha, FadeDuration);
		await ToSignal(tween, Tween.SignalName.Finished);
	}

	private async Task<PackedScene> LoadWithProgress(string path)
	{
		var loadTimes = new ConfigFile();
		var loadError = loadTimes.Load(LoadTimesFile);
		if (loadError != Error.Ok && loadError != Error.FileNotFound)
		{
			// Only affects the estimate – start fresh, the next save overwrites the broken file
			GD.PushWarning($"Could not read {LoadTimesFile}:  {loadError}");
			loadTimes = new ConfigFile();
		}
		var expected = (float)loadTimes.GetValue(LoadTimesSection, path, 1.0f);

		var requestError = ResourceLoader.LoadThreadedRequest(path, useSubThreads: true);
		if (requestError != Error.Ok)
		{
			GD.PushWarning($"Failed to start loading scene {path}:  {requestError}");
			return null;
		}
		
		_progressBar.Value = 0;
		var start = Time.GetTicksMsec();
		var progress = new Godot.Collections.Array();

		while (ResourceLoader.LoadThreadedGetStatus(path, progress) == ResourceLoader.ThreadLoadStatus.InProgress)
		{
			var elapsed = (Time.GetTicksMsec() - start) / 1000f;
			_progressBar.Visible = elapsed > ShowProgressAfter;
			
			// The time-based guess keeps the bar moving; it never claims the last 5 %
			var timeGuess = Mathf.Min(elapsed / expected, 0.95f);
			var target = Mathf.Max((float)progress[0], timeGuess) * 100;
			_progressBar.Value = Mathf.Lerp(_progressBar.Value, target, 0.2);

			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		}

		if (ResourceLoader.LoadThreadedGetStatus(path) != ResourceLoader.ThreadLoadStatus.Loaded)
		{
			GD.PushError($"Failed to load scene {path}");
			return null;
		}
		
		_progressBar.Value = 100;
		
		// Exponential moving average, so one unusually slow load doesn't skew the next estimate
		var measured = (Time.GetTicksMsec() - start) / 1000f;
		var average = loadTimes.HasSectionKey(LoadTimesSection, path)
			? Mathf.Lerp(expected, measured, 0.3f)
			: measured;
		loadTimes.SetValue(LoadTimesSection, path, average);
		
		var saveError = loadTimes.Save(LoadTimesFile);
		if (saveError != Error.Ok)
		{
			GD.PushWarning($"Could not save {LoadTimesFile}: {saveError}");
		}

		return (PackedScene)ResourceLoader.LoadThreadedGet(path);
	}
}
