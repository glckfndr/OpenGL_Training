#version 430

layout (location = 0) in vec3 VertexPosition;
layout (location = 1) in vec3 VertexNormal;
layout (location = 2) in vec2 VTexCoord;

out vec3 Position;
out vec3 Normal;
out vec2 TexCoord;

//uniform mat4 ModelViewMatrix;
uniform mat3 NormalMatrix;
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    Normal = normalize(NormalMatrix * VertexNormal);
    Position = vec3( view * model * vec4(VertexPosition,1.0));
    TexCoord = VTexCoord;

    gl_Position = projection * view * model * vec4(VertexPosition,1.0) ;
}
