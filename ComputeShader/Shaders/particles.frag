#version 430

in float Transp;
uniform sampler2D ParticleTex;


void main()
{
	gl_FragColor = texture(ParticleTex, gl_PointCoord);
	gl_FragColor.a = 1;
    gl_FragColor.a *= Transp;
	//gl_FragColor = vec4(1,0,0,1);
}