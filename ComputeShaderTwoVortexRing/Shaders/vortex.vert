#version 430

layout (location = 0) in vec4 VortexPosition;


uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    vec4 pos = vec4(VortexPosition.xyz,1);
    gl_Position =  projection * view * model *  pos ;
}
