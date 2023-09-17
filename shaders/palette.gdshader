shader_type canvas_item;

render_mode unshaded;

uniform sampler2D palette;

void fragment() {
	float c = textureLod(TEXTURE, UV, 0.0).r;
	// Assuming the screen texture is already grayscale,
	// there is no need to average color values.
	int i = int(round(c * 3.0));
	
	vec3 mapped = texelFetch(palette, ivec2(i, 0), 0).rgb;
	COLOR.rgb = mapped;
}
