extends TextureRect

@onready var mat: ShaderMaterial = self.material

func on_palette_changed(color_index: int = 0):
	print("on_palette_changed")
	if not mat:
		push_warning("Palette node does not have expected ShaderMaterial attached")
		return
	var p_tex = mat.get_shader_parameter("palette") as Texture2D
	if not p_tex:
		push_warning("Pallete node has no palette texture")
		return
	var bg_color = p_tex.get_image().get_pixel(color_index, 0)
	print("tex bg_color: ", bg_color)
#	print("old bg_color: ", ProjectSettings.get_setting("rendering/environment/defaults/default_clear_color"))
	print("old bg_color: ", RenderingServer.get_default_clear_color())
	RenderingServer.set_default_clear_color(bg_color)
	print("new bg_color: ", RenderingServer.get_default_clear_color())

# Called when the node enters the scene tree for the first time.
func _ready():
	mat.changed.connect(on_palette_changed)
	on_palette_changed(0)

func _unhandled_key_input(event):
	if event.is_action_pressed("e_key"):
		mat.set_shader_parameter("palette", null)
