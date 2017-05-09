#version 330
in vec4 color;
in vec3 normal;
out vec4 outputColor;
void main()
{
 vec3 lightSource = vec3(0.577, 0.577, 0.577);
 float f = abs( dot(lightSource, normal) );
 outputColor = color * f;
}