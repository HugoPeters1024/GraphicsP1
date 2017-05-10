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


void main()
{
 float a = (1.0/3.0) * (1 + sin((vPosition.x * 5) - animation));

  vec3 vPos = vec3(vPosition.x, vPosition.y, vPosition.z * a);

  gl_Position = M * vec4( vPos, 1.0 );
  color = vec4( vColor, 1.0);
  normal = vec3( (M * vec4( vNormal * a, 1.0) ) );
  vec3 lightDistance = Lpos - vPos;
  falloff = intensity/dot(lightDistance, lightDistance);
  lightDirection = normalize(lightDistance);
}