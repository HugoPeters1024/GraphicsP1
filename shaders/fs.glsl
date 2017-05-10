#version 330
in vec4 color;
in vec3 normal;
in vec3 lightDirection;
in float falloff;
out vec4 outputColor;

uniform vec3 Ldir;
uniform vec3 Lpos;

void main()
{
 float f = dot(lightDirection, normal);
 outputColor = color * f * sqrt(f) * falloff;
}