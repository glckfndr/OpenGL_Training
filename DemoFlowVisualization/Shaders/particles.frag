#version 430
in vec2 absVelocity;

uniform vec4 Color;

layout( location = 0 ) out vec4 FragColor;


void main() 
{
	float col = absVelocity.x;
	float k1 = step(col, 0.6);
	float k2 = step(0.6, col)*step(col, 0.8);
	float k3 = step(0.8, col)*step(col, 1.0);
	

	vec4 color3 = Color * 0 + vec4(0, 0.75 + col, 0.75 - col, 1);
	vec4 color1 = vec4(1 - col, 0, 0.75 + col, 1);
	vec4 color2 = vec4(0.75 + col, 1 - col, 0, 1);
		
	
	vec4 color4 = vec4(col + 0.5, col, col, 1);
	FragColor = k1*color1 + (1-k1)*k2*color2 + (1-k2)*k3*color3 + (1-k1)*(1-k2)*(1-k3)*color4;
	FragColor.a = 1.5 * absVelocity.y;
	
	
}