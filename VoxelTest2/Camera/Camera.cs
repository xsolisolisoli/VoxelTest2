using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace VoxelTest2.Camera
{
    public class Camera
    {
        public Vector3 _camPos;
        public  float _speed;
        private Vector3 _camTarget;
        private Vector3 _camDir;
        private Vector3 _camRight;

        //Direction Prims
        Vector3 front = new Vector3(0.0f, 0.0f, -1.0f);
        Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);

        private Vector3 _camUp;
        public Matrix4 _camView;


    public Camera()
    {
        _speed = 1.5f;
        _camPos = new Vector3(0, 0, 3);
        _camTarget = new Vector3(0, 0, 0);
        _camDir = Vector3.Normalize(_camPos - _camTarget);
        _camRight = Vector3.Normalize(Vector3.Cross(up, _camDir));
        _camUp = Vector3.Cross(_camDir, _camRight);
        _camView = Matrix4.LookAt(new Vector3(0.0f, 0.0f, 3.0f),
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector3(0.0f, 1.0f, 0.0f));
    }
    //= Vector3.Normalize(_camPos - _camTarget)
    protected void OnRender()
        {

        }
    public void ProcessInput(FrameEventArgs e, KeyboardState input)
        {
            if (input.IsKeyDown(Keys.W))
            {
                _camPos += front * _speed; //Forward 
            }

            if (input.IsKeyDown(Keys.S))
            {
                _camPos -= front * _speed; //Backwards
            }

            if (input.IsKeyDown(Keys.A))
            {
                _camPos -= Vector3.Normalize(Vector3.Cross(front, up)) * _speed; //Left
            }

            if (input.IsKeyDown(Keys.D))
            {
                _camPos += Vector3.Normalize(Vector3.Cross(front, up)) * _speed; //Right
            }

            if (input.IsKeyDown(Keys.Space))
            {
                _camPos += up * _speed; //Up 
            }

            if (input.IsKeyDown(Keys.LeftShift))
            {
                _camPos -= up * _speed; //Down
            }
            _camView = Matrix4.LookAt(_camPos, _camPos + front, up);
        }
    }
}