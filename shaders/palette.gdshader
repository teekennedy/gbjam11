shader_type canvas_item;

uniform sampler2D palette;
uniform sampler2D screen_texture : hint_screen_texture, filter_nearest, repeat_disable;

void fragment() {
	float c = textureLod(screen_texture, SCREEN_UV, 0.0).r;
	// Assuming the screen texture is already grayscale,
	// there is no need to average color values.
	int i = int(round(c * 3.0));
	
	vec3 mapped = texelFetch(palette, ivec2(i, 0), 0).rgb;
	COLOR.rgb = mapped;
//	COLOR.rgb = vec3(c_gray);
}