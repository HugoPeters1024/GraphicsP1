#version 330
in vec4 color;
in vec3 normal;
out vec4 outputColor;
void main()
{
 vec3 lightDir = vec3(0.577, 0.577, 0.577);
 float f = dot(lightDir, normal);
 outputColor = color * f;
}