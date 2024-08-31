using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Попытка_в_лучи
{
    public class Camera
    {
        public Vector2 pos;
        private float rotation;
        public float Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                if (value > 360) value -= 360;
                if (value < 0) value += 360;
                rotation = value;
            }
        }
        public float RadiansRotation
        {
            get
            {
                return Rotation / 180 * 3.14f;
            }
            private set
            {

            }
        }
        public float dof;
        public float fov;
        public Camera(Vector2 cameraPos, float camRotation, float fieldOfView, float depthOfField) 
        {
            pos = cameraPos;
            rotation = camRotation;
            fov = fieldOfView;
            dof = depthOfField;
        }
    }
}
