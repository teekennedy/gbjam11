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
		GetWindow().Size = new Vector2I(160 * 4, 144 * 4);

		_shader = (ShaderMaterial)Material;
		UpdateShaderPalette();
	}

	// Method to handle input that has not already been handled by the UI.
	public override void _UnhandledInput(InputEvent @event)
	{
		//had to get rid of @event due to events lot letting two happen at once
		if (Input.IsActionPressed("gb_up") && Input.IsActionPressed("gb_start"))
		{
			PalettePath = "res://assets/palettes/warm.png";
		}
		else if (Input.IsActionPressed("gb_down") && Input.IsActionPressed("gb_start"))
		{
			PalettePath = "res://assets/palettes/cool.png";
		}
		else if (Input.IsActionPressed("gb_left") && Input.IsActionPressed("gb_start") || Input.IsActionPressed("gb_right") && Input.IsActionPressed("gb_start"))
		{
			PalettePath = "res://assets/palettes/lcd.png";
		}
		else
		{
			// Return without marking input as handled
			return;
		}
		// Set input as handled so it doesn't get processed by other Nodes.
		//GetViewport().SetInputAsHandled();
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
