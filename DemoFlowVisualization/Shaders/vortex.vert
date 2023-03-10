#version 430

layout (location = 0) in vec4 VortexPosition;
layout (location = 1) in vec2 Velocity;
out float abs_velocity;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
	vec4 position = vec4(VortexPosition.xy,0,1);
    abs_velocity = length(position.xy); // for color of particles
    gl_Position =  projection * view * model *  position ;
}
