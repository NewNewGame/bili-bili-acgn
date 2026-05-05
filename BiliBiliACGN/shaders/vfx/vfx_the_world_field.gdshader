shader_type canvas_item;
render_mode blend_mix;

// 时停领域：屏幕采样 + 球形遮罩 + 向中心采样扭曲 + 负片感调色

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_linear_mipmap;

uniform vec2 u_viewport_size = vec2(1920.0, 1080.0);
uniform vec2 u_center_px = vec2(960.0, 540.0);
uniform float u_sphere_radius_px = 800.0;
uniform float u_edge_soft_px = 120.0;
uniform float u_distort_strength : hint_range(0.0, 1.0) = 0.42;
uniform float u_invert_strength : hint_range(0.0, 1.0) = 1.0;
// 轻微青金偏色（JoJo 时停氛围），在负片结果上混合
uniform float u_gold_tint : hint_range(0.0, 0.35) = 0.12;

void fragment() {
	vec2 px = SCREEN_UV * u_viewport_size;
	vec2 delta = px - u_center_px;
	float dist = length(delta);
	vec2 dir = dist > 0.5 ? normalize(delta) : vec2(0.0, 1.0);

	float denom = max(u_sphere_radius_px, 1.0);
	float norm = clamp(dist / denom, 0.0, 1.0);
	// 向中心扭曲：沿径向把采样点往中心收，外侧画面被“拉入”
	float pinch = u_distort_strength * pow(norm, 1.35);
	float sample_dist = clamp(dist * (1.0 - 0.52 * pinch), 0.0, length(u_viewport_size));
	vec2 px_sample = u_center_px + dir * sample_dist;
	vec2 suv = clamp(px_sample / u_viewport_size, vec2(0.001), vec2(0.999));

	vec4 scr = textureLod(screen_texture, suv, 0.0);
	vec3 inv = vec3(1.0) - scr.rgb;
	vec3 gold = inv * vec3(1.08, 0.96, 1.12);
	vec3 toned = mix(inv, gold, u_gold_tint);
	vec3 rgb = mix(scr.rgb, toned, u_invert_strength);

	float inner = smoothstep(u_sphere_radius_px, u_sphere_radius_px - u_edge_soft_px, dist);
	COLOR = vec4(rgb, inner);
}
