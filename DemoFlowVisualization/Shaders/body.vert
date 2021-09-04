#version 430
layout (location = 6) in vec2 VertexPosition;
out vec4 Color;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    gl_Position = projection * view * model * vec4(VertexPosition.xy, 0.1,  1.0);
	
	Color = vec4(1,0.5,0.2,1);
}
