#version 430
in float abs_velocity;

//uniform sampler2D Texture;
uniform vec4 Color;

layout( location = 0 ) out vec4 FragColor;

void main() 
{
  	FragColor = vec4(Color.r  * abs_velocity, Color.g  * (1- abs_velocity), 2* Color.b * abs_velocity,1.0);
	//FragColor = Color;
	
}