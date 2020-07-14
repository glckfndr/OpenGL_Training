#version 430
in vec3 VertexPosition;
in vec3 VertexColor;

out vec3 Color;

uniform float yCoord;
uniform float xCoord;

void main()
{
	Color = VertexColor;
	
	vec3 position = VertexPosition;
	position.y += yCoord;
	position.x += xCoord;
	Color.r = Color.r + position.y;
	gl_Position = vec4(position,1.0);
	
}