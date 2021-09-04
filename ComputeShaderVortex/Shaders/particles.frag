#version 430

in float absVelocity;
uniform vec4 Color;

layout( location = 0 ) out vec4 FragColor;


void main() 
{
	float k1 = step(absVelocity, 0.6);
	float k2 = step(0.6, absVelocity)*step(absVelocity, 0.8);
	float k3 = step(0.8, absVelocity)*step(absVelocity, 2.9);
	vec4 color3 = vec4(0.5, 0.0, 0, 0.5 + absVelocity);
	vec4 color2 = vec4(0.0, 0.4 + absVelocity, 0.0, 0.7);
	vec4 color1 = Color*absVelocity;
	color1 = vec4(0.0, 0, 0.2 + absVelocity, 0.7);
	vec4 color4 = vec4(absVelocity, absVelocity*0.5, 0.0, 1);
//	if(absVelocity  < 0.6)
//		FragColor = vec4(0.0, 0.0, 1.5*absVelocity, 1.5*absVelocity);
//	else if(absVelocity < 0.8)
//		FragColor = vec4(0.0, 1.25*absVelocity, 0.0, 0.7);
//	else if(absVelocity < 1.0)
//		FragColor = Color*absVelocity;
//	else
//		FragColor = vec4(absVelocity, absVelocity*0.5, 0.0, absVelocity*0.5);
FragColor = k1*color1 + (1-k1)*k2*color2 + (1-k2)*k3*color3 + (1-k1)*(1-k2)*(1-k3)*color4;
	
}