#version 330
in vec3 vPosition;
in vec3 vNormal;
in vec3 vColor;
out vec4 color;
out vec3 normal;
out vec3 lightDirection;
out float falloff;

uniform mat4 M;
uniform vec3 Ldir;
uniform vec3 Lpos;
uniform float intensity;
uniform float animation;
uniform float GoLoco; 


void main()
{
  float a;
  if(GoLoco == 2)
  {
    a = 0.5 * (1 + sin((vPosition.x * 5) - animation));
  }
  else if (GoLoco == 1)
  {
    a = 0.5 * sin(10 * gl_Position.x - animation) + 0.5;
  }
  else
  {
    a = 1;
  }

  vec3 vPos = vec3(vPosition.x, vPosition.y, vPosition.z * a);

  gl_Position = M * vec4( vPos, 1.0 );
  if(vPos.z > 0.6)
	{color = vec4( 1, 1, 1, 1);}
  else if (vPos.z > 0.16)
	{color = vec4( 0.5, 0.5, 0.5, 0.5);}
  else if (vPos.z > 0.015)
   	{color = vec4( vColor, 0.7);}
  else
	{color = vec4( 0, 0, 0.5, 1.0);}
  normal = max( vec3(0, 0, 0) , vec3( (M * vec4( vNormal * a, 1.0) ) ));
  vec3 lightDistance = Lpos - vPos;
  falloff = intensity/dot(lightDistance, lightDistance);
  lightDirection = normalize(lightDistance);
}