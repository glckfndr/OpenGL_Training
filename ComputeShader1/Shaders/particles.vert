#version 430

layout (location = 0) in vec4 VertexPosition;
layout (location = 1) in vec4 Velocity;

out vec3 Position;
out float Index;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    Position = (view * model * VertexPosition ).xyz;
	Index = Velocity.w;
    gl_Position =  projection * view * model *  VertexPosition ;
}
