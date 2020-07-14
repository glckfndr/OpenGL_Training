#version 330 core
in vec3 VertexPosition;
in vec2 TexturePosition;

out vec2 texCoord;

//uniform float Time;
uniform vec3 center0; // = vec3(0.0,0.0,0.0);
uniform vec3 center1; // = vec3(0.0,0.0,0.0);
uniform float gamma;
uniform float dt;


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
	vec3 gamma0 = vec3(0.0,0.0, gamma);
    vec3 gamma1 = vec3(0.0,0.0, -gamma);;
    
    texCoord = TexturePosition;
	//vec3 Center0 = center0;
	//Center0.y += 0.5*sin(Time*1.5);
	
	vec3 vel = velocity(VertexPosition, center0, gamma0) + velocity(VertexPosition, center1, gamma1);
	
    gl_Position = vec4(VertexPosition + vel * dt, 1.0);
}

