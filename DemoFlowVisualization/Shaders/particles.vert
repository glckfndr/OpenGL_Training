#version 430

layout (location = 0) in vec4 VertexPosition;
layout (location = 1) in vec4 Velocity;

out vec2 absVelocity;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    absVelocity = vec2((length(Velocity)-0.9)*2, Velocity.z);
    //gl_Position =  projection * view * model *  VertexPosition ;
	gl_Position =  projection * view * model * vec4( VertexPosition.xy, 0, 1) ;
}
