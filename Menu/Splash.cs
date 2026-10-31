using Godot;

public partial class Splash : Control
{
    private const string NextScene = "res://Menu/MainMenu.tscn";

    private bool _leaving;

    public override async void _Ready()
    {
        // Rotate the tilted logo around its centre, whatever size the text ends up
        var tilt = GetNode<Control>("%CenterContainer");
        tilt.PivotOffset = tilt.Size / 2;
        tilt.Resized += () => tilt.PivotOffset = tilt.Size / 2;

        await GetNode<DecodeLabel>("%Line1").Play();
        if (_leaving)
        {
            return;
        }

        await GetNode<DecodeLabel>("%Line2").Play();
        if (_leaving)
        {
            return;
        }

        await ToSignal(GetTree().CreateTimer(1.5), SceneTreeTimer.SignalName.Timeout);
        Leave();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // Any key, click or controller button skips the splash
        if (@event.IsPressed() && !@event.IsEcho())
        {
            GetViewport().SetInputAsHandled();
            Leave();
        }
    }

    private void Leave()
    {
        if (_leaving)
        {
            return;
        }

        _leaving = true;
        GetNode<SceneTransition>("/root/SceneTransition").ChangeScene(NextScene);
    }
}
