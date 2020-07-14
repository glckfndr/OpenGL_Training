#version 430

layout (location = 0) in vec4 VertexPosition;

out vec3 Position;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    Position = (VertexPosition * model * view).xyz;
    gl_Position = VertexPosition * model * view * projection;
}
