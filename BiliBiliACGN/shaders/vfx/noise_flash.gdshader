shader_type canvas_item;

uniform vec4 flash_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform float speed : hint_range(0.0, 10.0) = 2.0;
uniform float frequency : hint_range(1.0, 200.0) = 60.0; // 越大越“细”
uniform float softness : hint_range(0.001, 0.5) = 0.06;   // 越小亮带越窄
uniform float alpha_gain : hint_range(0.0, 2.0) = 1.0;
uniform float warp_amount : hint_range(0.0, 1.0) = 0.18;  // 曲线弯曲程度（越大越弯）
uniform float warp_scale : hint_range(0.2, 20.0) = 2.0;   // 曲线尺度（越大弯曲变化越频繁）
uniform float warp_speed : hint_range(0.0, 10.0) = 0.35;  // 曲线滚动速度

float hash21(vec2 p) {
	// 简单 hash：0..1
	p = fract(p * vec2(123.34, 456.21));
	p += dot(p, p + 78.233);
	return fract(p.x * p.y);
}

float value_noise(vec2 p) {
	// 2D value noise（0..1）
	vec2 i = floor(p);
	vec2 f = fract(p);
	// 平滑插值
	vec2 u = f * f * (3.0 - 2.0 * f);

	float a = hash21(i + vec2(0.0, 0.0));
	float b = hash21(i + vec2(1.0, 0.0));
	float c = hash21(i + vec2(0.0, 1.0));
	float d = hash21(i + vec2(1.0, 1.0));

	return mix(mix(a, b, u.x), mix(c, d, u.x), u.y);
}

float fbm(vec2 p) {
	// 分形布朗噪声：更平滑、连续（0..1）
	float v = 0.0;
	float a = 0.55;
	for (int i = 0; i < 3; i++) {
		v += a * value_noise(p);
		p = p * 2.0 + vec2(17.0, 31.0);
		a *= 0.5;
	}
	return v;
}

void fragment() {
	// 左下->右上方向：用 (UV.x - UV.y) 作为对角线坐标
	float diag = UV.x - UV.y;
	float t = TIME * speed;

	// 关键：不要直接“加噪声到亮度/阈值”，会导致亮带断裂。
	// 改成对对角线坐标做平滑的相位偏移（warp），亮带会变成连续的弯曲曲线。
	float w = fbm(UV * warp_scale + vec2(TIME * warp_speed, -TIME * warp_speed));
	float diag_warped = diag + (w * 2.0 - 1.0) * warp_amount;

	// 0..1 波（在 warped 坐标上形成连续曲线）
	float wave = 0.5 + 0.5 * sin(diag_warped * frequency - t);

	// 生成窄亮带（接近 wave=1 时为 1，否则趋近 0）
	float band = smoothstep(1.0 - softness, 1.0, wave);

	// 没有波纹处 alpha=0
	float a = clamp(band * alpha_gain, 0.0, 1.0);

	COLOR = vec4(flash_color.rgb, a);
}