#version 430
layout (location = 0) in vec3 VertexPosition;
layout (location = 1) in vec3 VertexColor;

out vec3 Color;
out vec3 TexCoord;

uniform mat4 RotationMatrix;

void main()
{
	Color = VertexColor;
	TexCoord = VertexPosition;
	gl_Position = RotationMatrix * vec4(VertexPosition,1.0);
	
}