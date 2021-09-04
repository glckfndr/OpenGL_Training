#version 410
const float PI = 3.14159265359;

layout (location = 1) in vec4 Vortex;

// Out to fragment shader
out vec2 TexCoord;          // Texture coordinate
uniform float ParticleSize = 0.1;       // Size of particle

uniform mat4 model;			// Projection matrix
uniform mat4 view;			// Projection matrix
uniform mat4 projection;	// Projection matrix

// Offsets to the position in camera coordinates for each vertex of the particle's quad
const vec3 offsets[] = vec3[](vec3(-0.5,-0.5,0), vec3(0.5,-0.5,0), vec3(0.5,0.5,0),
                              vec3(-0.5,-0.5,0), vec3(0.5,0.5,0), vec3(-0.5,0.5,0) );
// Texture coordinates for each vertex of the particle's quad
const vec2 texCoords[] = vec2[](vec2(0,0), vec2(1,0), vec2(1,1), vec2(0,0), vec2(1,1), vec2(0,1));

void render() 
{
     //vec3 posCam = vec3(0.0);
     vec3 posCam = (view * model * vec4(Vortex.xy,0,1)  ).xyz + offsets[gl_VertexID] * ParticleSize;   
    TexCoord = texCoords[gl_VertexID];
	gl_Position = projection * vec4(posCam,1);
//	gl_Position = projection *  (view * model * 
//							(vec4(Vortex.xy, 0.0, 1.0) + vec4(offsets[gl_VertexID], 1.0) * ParticleSize));
		//gl_Position = projection *  view * model * vec4(Vortex.xy, 0.0, 1.0);
}

void main() 
{
    render();
}
