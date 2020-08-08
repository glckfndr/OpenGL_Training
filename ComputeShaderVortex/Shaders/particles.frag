#version 430

in float absVelocity;
uniform vec4 Color;

layout( location = 0 ) out vec4 FragColor;


void main() 
{
	if(absVelocity  < 0.5)
		FragColor = vec4(0.0, 0.0, 0.8, 0.5);
	else if(absVelocity < 0.8)
		FragColor = vec4(0.0, 0.7, 0.0, 0.7);
	else if(absVelocity < 1.0)
		FragColor = Color;
	else
		FragColor = vec4(0.9, 0.0, 0.0, 1);
	
}