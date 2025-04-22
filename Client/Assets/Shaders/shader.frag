#version 330 core

uniform vec4 objectColor;

out vec4 FragColor;

void main()
{
    FragColor = objectColor;
}