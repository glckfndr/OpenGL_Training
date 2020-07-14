#version 400

layout (location = 0 ) in vec3 VertexPosition;
layout (location = 1) in vec3 VertexNormal;
layout (location = 2) in vec2 VertexTexCoord;

out vec4 Position;
out vec3 Normal;
out vec2 TexCoord;

uniform float Time;
uniform float Freq = 2.5;
uniform float Velocity = 2.5;
uniform float Amp = 0.6;

uniform mat4 ModelViewMatrix;
uniform mat3 NormalMatrix;
uniform mat4 MVP;

void main()
{
    vec4 pos = vec4(VertexPosition,1.0);

    float u = Freq * pos.x + Freq * pos.z - Velocity * Time;
	
    pos.y = Amp * sin( u );

    
    vec3 n = normalize(vec3(cos( u ), 1, cos( u )));

    Position = pos * ModelViewMatrix ;
    Normal = n * NormalMatrix  ;
    TexCoord = VertexTexCoord;
    gl_Position =  pos * MVP ;
}
