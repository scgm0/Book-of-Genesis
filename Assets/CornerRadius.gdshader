shader_type canvas_item;

uniform float corner_scale: hint_range(0., 1.) = 0.;

float roundedBoxSDF(vec2 CenterPosition, vec2 Size, float Radius) {
    return length(max(abs(CenterPosition) - Size + Radius, 0.0)) - Radius;
}
void fragment() {
	vec2 uv = (UV * 2.0) - 1.0;
	float m = roundedBoxSDF(uv, vec2(1.0), corner_scale + 0.000001);//length(max(abs(uv)-0.79, 0.0));
    float c = smoothstep(0.0, 0.0 - fwidth(m) * 2.0, m);
    COLOR.a =  COLOR.a * c;
}