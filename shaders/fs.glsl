#version 330
in vec4 color;
in vec3 normal;
out vec4 outputColor;
void main()
{
 vec3 lightSource = vec3(0.707, 0, 0.707);
 float f = dot(lightSource, normal);
 outputColor = color * f;
}