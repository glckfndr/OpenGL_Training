#version 400

layout (location = 0 ) in vec3 VertexPosition;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    gl_Position =  vec4(VertexPosition, 1.0) * model * view * projection;
}
