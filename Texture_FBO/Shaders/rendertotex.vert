#version 410

layout (location = 0) in vec3 VertexPosition;
layout (location = 1) in vec3 VertexNormal;
layout (location = 2) in vec2 VertexTexCoord;

out vec3 Position;
out vec3 Normal;
out vec2 TexCoord;

uniform mat4 ModelViewMatrix;
uniform mat3 NormalMatrix;
uniform mat4 ProjectionMatrix;
uniform mat4 MVP;

void main()
{
    TexCoord = VertexTexCoord;
    Normal = normalize(  VertexNormal * NormalMatrix );
    Position = vec3(  vec4(VertexPosition,1.0) * ModelViewMatrix );

    gl_Position =  vec4(VertexPosition,1.0) * MVP;
}
