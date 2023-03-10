#version 430

layout (location = 0) in vec4 particlePositionBuf;
layout (location = 2) in vec4 particleVelBuf;

out vec2 absVelocity;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    //absVelocity = vec2((length(Velocity)-0.9)*2, Velocity.z);
	absVelocity = vec2(length(particleVelBuf.xy), particleVelBuf.z);
  //  gl_Position =  projection * view * model *  VertexPosition ;
	gl_Position =  projection * view * model * vec4( particlePositionBuf.xy, 0, 1) ;
}
