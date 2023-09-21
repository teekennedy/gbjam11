using Godot;

public partial class Palette : SubViewportContainer
{
    // Reference to this node's Shader.
    private ShaderMaterial _shader;

    // Path to the 4x1 png representing the color palette for the game.
    // This can be set at runtime to change the color palette dynamically.
    private string _palettePath = "res://assets/palettes/lcd.png";
    public string PalettePath
    {
        get => _palettePath;
        set
        {
            _palettePath = value;
            UpdateShaderPalette();
        }
    }


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

        _shader = (ShaderMaterial)Material;
        UpdateShaderPalette();
    }

    // Updates the palette used by the shader to the resource at PalettePath.
    // Called internally whenever the PalettePath attribute is changed.
    private void UpdateShaderPalette()
    {
        var texture = ResourceLoader.Load<CompressedTexture2D>(_palettePath, "CompressedTexture2D");
        _shader.SetShaderParameter("palette", texture);
    }

    // Convenience function to get the palette color at a specific index.
    // Returns null if no palette is currently in use.
    // use RenderingServer.SetDefaultClearColor(color) to set this color as the default background color.
    public Color? GetPaletteColor(int index = 0)
    {
        // Get the current palette texture from the shader
        var palette = _shader.GetShaderParameter("palette").As<GodotObject>();
        if (palette is CompressedTexture2D)
        {
            var tex = palette as CompressedTexture2D;
            // Grab the color of the pixel at index, 0.
            return tex.GetImage().GetPixel(index, 0);
        }
        return null;
    }
}
