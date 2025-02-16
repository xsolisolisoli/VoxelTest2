#version 330 core
                in vec3 vNormal;
                in vec4 vColor;

                out vec4 FragColor;

                void main()
                {
                    // Simple directional lighting
                    vec3 lightDir = normalize(vec3(0.5, 1.0, 0.7));
                    float diff = max(dot(vNormal, lightDir), 0.2);
                    FragColor = vColor * vec4(vec3(diff), 1.0);
                }