shader_type canvas_item;

uniform float duration : hint_range(0.05, 10.0, 0.01) = 1.0;
uniform float max_scale : hint_range(1.0, 100.0, 0.01) = 50.0;
uniform vec4 color_start : source_color = vec4(0.58, 0.24, 0.92, 0.1);
uniform vec4 color_end : source_color = vec4(0.0, 0.0, 0.0, 0.7);
/// 小于 0 时用 TIME 按 duration 循环；0–1 为手动进度（脚本里可逐帧设置）
uniform float progress : hint_range(-1.0, 1.0, 0.001) = -1.0;

varying float v_t;

void vertex() {
	v_t = progress >= 0.0 ? clamp(progress, 0.0, 1.0) : fract(TIME / duration);
	float s = mix(1.0, max_scale, v_t);
	VERTEX *= s;
}

void fragment() {
	vec4 modu = COLOR;
	vec4 c = texture(TEXTURE, UV);
	float m = c.r;
	vec4 grad = mix(color_start, color_end, v_t);
	COLOR.rgb = grad.rgb * m * modu.rgb;
	COLOR.a = m * grad.a * modu.a;
}
