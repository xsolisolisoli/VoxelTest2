#version 330 core

                layout(location = 0) in vec3 aPosition;
                layout(location = 1) in vec3 aNormal;
                layout(location = 2) in vec4 aColor;

                uniform mat4 mvp;

                out vec3 vNormal;
                out vec4 vColor;

                void main()
                {
                    gl_Position = mvp * vec4(aPosition, 1.0);
                    vNormal = aNormal;
                    vColor = aColor;
                }