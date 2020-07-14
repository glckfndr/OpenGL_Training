#version 430

in float Transp;
uniform sampler2D ParticleTex;

//layout ( location = 0 ) out vec4 FragColor;

void main()
{
	gl_FragColor = texture(ParticleTex, gl_PointCoord);
	gl_FragColor.a = 1;
    gl_FragColor.a *= Transp;
}