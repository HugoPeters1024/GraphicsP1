#version 330
in vec4 color;
in vec3 normal;
out vec4 outputColor;
void main()
{
 vec3 lightSource = vec3(1, 1, 1);
 float f = dot(lightSource, normal);
 outputColor = color * f;
}