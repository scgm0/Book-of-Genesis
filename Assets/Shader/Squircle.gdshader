shader_type canvas_item;

uniform float power = 4.0;

float Squircle(vec2 uv, float r, float n, float blur) {
    float c = pow(abs(uv.x),n) + pow(abs(uv.y),n);
    return smoothstep(r+blur,r-blur,c);
}

void fragment() {
	vec2 uv = (UV * 2.0) - 1.0;
	float cov1 = Squircle(uv, 1.0, power, 0.25);
	COLOR.a = COLOR.a * cov1;
}
