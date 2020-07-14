#version 430

uniform sampler2D ParticleTex;

in float Transp;

layout ( location = 0 ) out vec4 FragColor;

void main()
{

    FragColor = texture(ParticleTex, gl_PointCoord);
    FragColor = vec4(mix( vec3(0,0,0), FragColor.xyz, Transp ), FragColor.a); 
    FragColor.a *= Transp;
	//FragColor = vec4(1,0,0,1);
	//FragColor = vec4(mix( vec3(0,0,0), FragColor.xyz, Transp ), FragColor.a); 
	
	
}
