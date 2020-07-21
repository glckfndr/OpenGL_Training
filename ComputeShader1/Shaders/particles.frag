#version 430

in vec3 Position;
in float Index;

uniform vec4 Color;

layout( location = 0 ) out vec4 FragColor;


void main() 
{
  if(Index == 1.0)
	FragColor = vec4(0,0.8,0,0.8);
  else if(Index == 0.5)
	FragColor = vec4(0,0,0.8,0.8);
	else
	FragColor = Color;//*0 + vec4(1,1,0,1); ;
	
}