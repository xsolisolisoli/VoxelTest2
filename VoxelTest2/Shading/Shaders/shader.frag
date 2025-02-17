#version 330 core

in vec3 FragPos;
in vec3 Normal;
in vec4 Color;

out vec4 FragColor;

void main()
{
    // Simple lighting calculation
    vec3 lightPos = vec3(1.2f, 1.0f, 2.0f);
    vec3 lightColor = vec3(1.0f, 1.0f, 1.0f);
    vec3 lightDir = normalize(lightPos - FragPos);
    float diff = max(dot(Normal, lightDir), 0.0);
    vec3 diffuse = diff * lightColor;

    vec3 result = (diffuse + vec3(0.1)) * Color.rgb; // Adding ambient light
    FragColor = vec4(result, Color.a);
}