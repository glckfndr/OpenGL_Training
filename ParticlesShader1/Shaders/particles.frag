#version 410

uniform sampler2D ParticleTexture;

in float Transp;
in vec2 TexCoord;

layout ( location = 0 ) out vec4 FragColor;

void main()
{
    FragColor = texture(ParticleTexture, TexCoord);
    FragColor.a *= Transp;
}
