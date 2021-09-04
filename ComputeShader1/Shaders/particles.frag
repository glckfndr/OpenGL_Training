#version 430

in vec3 Position;
in float Index;

uniform vec4 Color;

layout( location = 0 ) out vec4 FragColor;
//rgb(0.27, 1.00, 0.00)
//rgb(0.12, 0.67, 0.05)

void main() 
{
  if(Index == 1.0)
	FragColor = vec4(0.27, 1.0, 0.6, 0.7);
  else if(Index == 0.5)
	FragColor = vec4(0.5, 0.67, 0.05, 0.8);
	else
	FragColor = Color;//*0 + vec4(1,1,0,1); ;
	
}