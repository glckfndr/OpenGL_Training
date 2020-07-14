#version 430

layout (location = 0) in vec3 InitialVelocity; // Particle initial velocity
layout (location = 1) in float StartTime;    // Particle "birth" time

out float Transp;  // Transparency of the particle

uniform float Time;  // Animation time
uniform vec3 Gravity = vec3(0.0,-0.05,0.0);  // world coords
uniform float ParticleLifetime;  // Max particle lifetime

uniform mat4 MVP;

void main()
{
    // Assume the initial position is (0,0,0).
    vec3 pos = vec3(0.0);
    Transp = 0.0;

    // Particle dosen't exist until the start time
    if( Time > StartTime ) 
	{
        float t = Time - StartTime;

        if( t < ParticleLifetime ) 
		{
            pos = InitialVelocity * t + Gravity * t * t / 2;
            Transp = clamp(1.0 - t / ParticleLifetime,0.0, 1.0);
			
        }
    }

    // Draw at the current position
    gl_Position =  vec4(pos, 1.0) * MVP ;
	
}
