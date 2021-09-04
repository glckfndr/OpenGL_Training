#version 430

layout (location = 0) in vec4 VertexPosition;
layout (location = 1) in vec4 Velocity;

out float absVelocity;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    absVelocity = length(Velocity/10.0);
    gl_Position =  projection * view * model *  VertexPosition ;
}
