using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Попытка_в_лучи
{
    public class Camera : IThinker, IMovable
    {
        private float rotation;
        public Vector2 Position { get; set; }
        public List<Vector2> Vision { get; set; } = new List<Vector2>();
        public List<Vector2> Other { get; set; } = new List<Vector2>();
        public float StartAngle { get; private set; }
        public float EndAngle { get; private set; }
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
                StartAngle = value - dof / 2;
                EndAngle = value + dof / 2;
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
        private float speed = 1f;
        public Camera(Vector2 cameraPos, float camRotation, float fieldOfView, float depthOfField) 
        {
            Position = cameraPos;
            rotation = camRotation;
            fov = fieldOfView;
            StartAngle = Rotation - fov / 2;
            EndAngle = Rotation + fov / 2;
            dof = depthOfField;
            Program.thinkers.Add(this);
        }
        public void Think()
        {
            RayCast rayCast = new RayCast(0.0005f, dof, ref Program.field);
            Vision = rayCast.HitsAsync(Position, StartAngle, EndAngle, 5f).Result.ToList();
            Other = rayCast.HitsAsync(Position, 0f, 360f, 7f).Result.ToList();
            //Rotation -= 15f;
            Program.hits = Vision;
            Program.collisionHits = Other;
            Move(CalcPosition());
        }
        private Vector2 CalcPosition()  //Вроде работает, только медленно пздц.
        {
            switch(Controller.pressedKey)
            {
                case Controller.Key.W:
                    return new Vector2(Position.x, Position.y, Position.x, Position.y - speed).Rotate(Rotation - 270f);
                case Controller.Key.A:
                    return new Vector2(Position.x, Position.y, Position.x, Position.y - speed).Rotate(Rotation - 180f);
                case Controller.Key.S:
                    return new Vector2(Position.x, Position.y, Position.x, Position.y - speed).Rotate(Rotation - 90f);
                case Controller.Key.D:
                    return new Vector2(Position.x, Position.y, Position.x, Position.y - speed).Rotate(Rotation);
                case Controller.Key.LeftArrow:
                    Rotation += 10f;
                    break; 
                case Controller.Key.RightArrow:
                    Rotation -= 10f;
                    break;
            }
            return new Vector2();
        }
        public void Move(Vector2 where)
        {
            Position += where;
        }
    }
}
