#version 430
in vec3 Color;
in vec3 TexCoord;
layout (location = 0) out vec4 FragColor;

struct BlobSettings
{
	vec4 InnerColor;
	vec4 OuterColor;
	float RadiusInner;
	float RadiusOuter;
};

uniform BlobSettings blob;

void main() 
{
    float dx = TexCoord.x - 0.2;
	float dy = TexCoord.y - 0.2;
	float dist = sqrt(dx * dx + dy * dy);
	FragColor = mix( blob.InnerColor, blob.OuterColor, smoothstep( blob.RadiusInner, blob.RadiusOuter, dist ));
	FragColor +=Color;
}