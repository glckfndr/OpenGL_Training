#version 330 core
layout (location = 0 ) in vec3 VertexPosition;

out vec4 Position;
out vec3 Normal;

uniform float Time;
uniform float Freq = 2.0;
uniform float Velocity = 0.2f;
uniform float Amp = 0.3;

uniform mat4 ModelViewMatrix;
uniform mat3 NormalMatrix;
uniform mat4 MVP;

void main()
{
    vec4 pos = vec4(VertexPosition, 1.0);
	float K = Freq/Velocity;
    float u = K * pos.x - Freq * Time;
    pos.y += Amp * sin(u);
	Position = ModelViewMatrix * pos;
    gl_Position = pos;
    Position = pos;

	vec3 n = vec3(0.0);
    n.xy = normalize(-K * Amp * vec2(cos(u), 1.0));

	Normal = NormalMatrix * n;

}
