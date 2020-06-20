#version 430
layout (location = 0) in vec3 VertexPosition;
layout (location = 1) in vec3 VertexColor;
out vec3 Color;
uniform float yCoord;

void main()
{
	Color = VertexColor;
	
	vec3 position = VertexPosition;
	position.y += yCoord;
	Color.r = Color.r + position.y;
	gl_Position = vec4(position,1.0);
	
}