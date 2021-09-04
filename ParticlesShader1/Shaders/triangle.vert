#version 410
const float PI = 3.14159265359;

layout (location = 0) in vec3 VertexPosition;


// Out to fragment shader
out vec2 TexCoord;          // Texture coordinate

uniform float ParticleSize;	// Size of particle

uniform mat4 model;			
uniform mat4 view;			
uniform mat4 projection;	

const float w = 0.3;
const float h = 0.3;

// Offsets to the position in camera coordinates for each vertex of the particle's quad
const vec3 offsets[] = vec3[](vec3(-w, -h, 0), vec3(w, -h, 0), vec3(w, h, 0),
                              vec3(-w, -h, 0), vec3(w, h, 0), vec3(-w, h, 0) );
// Texture coordinates for each vertex of the particle's quad
const vec2 texCoords[] = vec2[](vec2(0, 0), vec2(1, 0), vec2(1, 1), 
								vec2(0, 0), vec2(1, 1), vec2(0, 1));
								
void main() 
{
mat4 ggg = model;
    vec3	posCam = (view * vec4(-2, 2 + 0.5*gl_InstanceID, 0, 1)).xyz + offsets[gl_VertexID] * ParticleSize;
    TexCoord = texCoords[gl_VertexID];
	gl_Position = projection * vec4(posCam,1);
}
