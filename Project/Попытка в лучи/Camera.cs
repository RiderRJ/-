using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Попытка_в_лучи
{
    public class Camera : IThinker
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
        public List<Vector2> vision { get; set;  } = new List<Vector2>();
        public List<Vector2> other { get; set; } = new List<Vector2>();
        public float startAngle;
        public float endAngle;
        public Camera(Vector2 cameraPos, float camRotation, float fieldOfView, float depthOfField) 
        {
            pos = cameraPos;
            rotation = camRotation;
            fov = fieldOfView;
            dof = depthOfField;
            Program.thinkers.Add(this);
        }
        public void Think()
        {
            RayCast rayCast = new RayCast(0.0005f, dof, ref Program.field);
            vision = rayCast.HitsAsync(pos, startAngle, endAngle, 5f).Result.ToList();
            other = rayCast.HitsAsync(pos, 0f, 360f, 7f).Result.ToList();
            Rotation -= 15f;
            startAngle = Rotation - fov / 2f;
            endAngle = Rotation + fov / 2f;
            Program.hits = vision;
            Program.collisionHits = other;
        }
    }
}
