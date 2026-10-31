using System.Text;
using System.Threading.Tasks;
using Godot;

public partial class DecodeLabel : Label
{
    private const string Glyphs = "0123456789ABCDEF#$%&*+=<>?";

    [Export] public float CharDelay = 0.07f;
    [Export] public int ScrambleSteps = 4;

    private string _target;

    public override void _Ready()
    {
        // Resolve the translation once, then stop auto-translating the scrambled text
        _target = Tr(Text);
        AutoTranslateMode = AutoTranslateModeEnum.Disabled;
        Text = "";

        // Reserve the final size up front, so the block doesn't shift while typing
        var font = GetThemeFont("font");
        var fontSize = GetThemeFontSize("font_size");
        CustomMinimumSize = font.GetStringSize(_target, HorizontalAlignment.Left, -1, fontSize);
    }

    public async Task Play()
    {
        var shown = new StringBuilder();

        foreach (var character in _target)
        {
            if (!char.IsWhiteSpace(character))
            {
                for (var i = 0; i < ScrambleSteps; i++)
                {
                    Text = shown.ToString() + Glyphs[GD.RandRange(0, Glyphs.Length - 1)];
                    await ToSignal(GetTree().CreateTimer(CharDelay / ScrambleSteps), SceneTreeTimer.SignalName.Timeout);

                    if (!IsInsideTree())
                    {
                        return; // splash was skipped and freed mid-animation
                    }
                }
            }

            shown.Append(character);
            Text = shown.ToString();
        }
    }
}
