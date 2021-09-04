#version 430

in vec3 Position;
in vec3 Normal;
in vec2 TexCoord;

uniform vec4 LightPosition;
uniform vec3 LightIntensity;

uniform vec3 Kd;            // Diffuse reflectivity
uniform vec3 Ka;            // Ambient reflectivity
uniform vec3 Ks;            // Specular reflectivity
uniform float Shininess;    // Specular shininess factor

layout( binding = 0 ) uniform sampler2D Texture;

layout( location = 0 ) out vec4 FragColor;

vec3 ads( )
{
    vec3 s = normalize( vec3(LightPosition) - Position );
    vec3 v = normalize(vec3(-Position));
    vec3 r = reflect( -s, Normal );

    return
        LightIntensity * ( Ka +
          Kd * max( dot(s, Normal), 0.0 ) +
          Ks * pow( max( dot(r,v), 0.0 ), Shininess ) );
}

vec3 phongModel( vec3 pos, vec3 norm )
{
    vec3 s = normalize(vec3(LightPosition) - pos);
    vec3 v = normalize(-pos.xyz);
    vec3 r = reflect( -s, norm );
    vec3 ambient = LightIntensity * Ka;
    float sDotN = max( dot(s,norm), 0.0 );
    vec3 diffuse = LightIntensity * Kd * sDotN;
    vec3 spec = vec3(0.0);
    if( sDotN > 0.0 )
        spec = LightIntensity * Ks * pow( max( dot(r,v), 0.0 ), Shininess );

    return ambient + diffuse + spec;
}

void main() 
{
  vec4 texColor = texture( Texture, TexCoord );
  FragColor = vec4(ads(), 1.0) * texColor;
  
}
