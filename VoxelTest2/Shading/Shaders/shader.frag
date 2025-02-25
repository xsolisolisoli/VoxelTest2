#version 330 core

in vec3 FragPos;
in vec3 Normal;
in vec4 Color;

out vec4 FragColor;

void main()
{
    // Get the screen coordinates of the fragment
    vec2 screenPos = gl_FragCoord.xy;

    // Define the center of the screen and the size of the square
    vec2 screenSize = vec2(800.0, 600.0); // Replace with your actual screen size
    vec2 center = screenSize / 2.0;
    float squareSize = 50.0; // Size of the square in pixels

    // Check if the fragment is within the square
    if (abs(screenPos.x - center.x) < squareSize / 2.0 && abs(screenPos.y - center.y) < squareSize / 2.0)
    {
        FragColor = vec4(1.0, 0.0, 0.0, 1.0); // Red color for the square
    }
    else
    {
        // Simple lighting calculation for other fragments
        vec3 lightPos = vec3(1.2f, 1.0f, 2.0f);
        vec3 lightColor = vec3(1.0f, 1.0f, 1.0f);
        vec3 lightDir = normalize(lightPos - FragPos);
        float diff = max(dot(Normal, lightDir), 0.0);
        vec3 diffuse = diff * lightColor;

        vec3 result = (diffuse + vec3(0.1)) * Color.rgb; // Adding ambient light
        FragColor = vec4(result, Color.a);
    }
}