#version 330 core

layout(location = 0) in vec3 VertexPosition;

// We add another input variable for the texture coordinates

layout(location = 1) in vec2 TexturePosition;

// ...However, they aren't needed for the vertex shader itself.
// Instead, we create an output variable so we can send that data to the fragment shader.

out vec2 texCoord;
//uniform float Time;
uniform vec3 center0; // = vec3(0.0,0.0,0.0);
uniform vec3 center1; // = vec3(0.0,0.0,0.0);
uniform vec3 gamma0 = vec3(0.0,0.0, 0.004);
uniform vec3 gamma1 = vec3(0.0,0.0, -0.004);;

vec3 velocity(vec3 r, vec3 center, vec3 gamma)
{
	float r2 = dot(r - center, r - center);
	if(r2 < 0.01)	r2 = 0.01;
	return cross(r - center, gamma)/r2;
}

void main(void)
{
    // Then, we further the input texture coordinate to the output one.
    // texCoord can now be used in the fragment shader.
    
    texCoord = TexturePosition;
	//vec3 Center0 = center0;
	//Center0.y += 0.5*sin(Time*1.5);
	
	vec3 vel = velocity(VertexPosition, center0, gamma0) + velocity(VertexPosition, center1, gamma1);
	
    gl_Position = vec4(VertexPosition + vel, 1.0);
}

