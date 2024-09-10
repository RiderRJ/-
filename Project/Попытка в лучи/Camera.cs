using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Threading.Tasks;

namespace Попытка_в_лучи
{
    public class Camera : Thinker, IMovable
    {
        public override List<Vector2> Vision { get; set; } = new List<Vector2>();
        public override List<Vector2> Other { get; set; } = new List<Vector2>();
        public Vector2 Position { get; set; }
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
        private float rotation;
        public float dof;//depth of field
        public float fov;//field of view
        private float speed = 1f;
        private float scaleX = 2f;
        private float scaleY = 2f;
        public Camera(Vector2 cameraPos, float camRotation, float fieldOfView, float depthOfField) 
        {
            Position = cameraPos;
            rotation = camRotation;
            fov = fieldOfView;
            StartAngle = Rotation - fov / 2;
            EndAngle = Rotation + fov / 2;
            dof = depthOfField;
        }
        public override void Think()
        {
            RayCast rayCast = new RayCast(0.0005f, dof, ref Program.field);
            Vision = rayCast.HitsAsync(Position, StartAngle, EndAngle, 0.5f).Result.ToList();
            Other = rayCast.HitsAsync(Position, 0f, 360f, 7f).Result.ToList();
            Program.hits = Vision;
            Program.collisionHits = Other;
            Move(CalcPosition());
        }
        private Vector2 CalcPosition()  //Вроде работает, только медленно пздц.
        {
            Vector2 nPosition = new Vector2();
            switch(Controller.pressedKey)
            {
                case Controller.Key.W:
                    nPosition = new Vector2(Position.x, Position.y, Position.x, Position.y - speed).Rotate(Rotation - 270f);
                    break;
                case Controller.Key.A:
                    nPosition = new Vector2(Position.x, Position.y, Position.x, Position.y - speed).Rotate(Rotation - 180f);
                    break;
                case Controller.Key.S:
                    nPosition = new Vector2(Position.x, Position.y, Position.x, Position.y - speed).Rotate(Rotation - 90f);
                    break;
                case Controller.Key.D:
                    nPosition = new Vector2(Position.x, Position.y, Position.x, Position.y - speed).Rotate(Rotation);
                    break;
                case Controller.Key.LeftArrow:
                    Rotation += 10f;
                    break; 
                case Controller.Key.RightArrow:
                    Rotation -= 10f;
                    break;
            }
            //проверить коллизию квадрата в этом месте, свиднуть на разницу
            return nPosition;
        }
        public void Move(Vector2 where)
        {
            Position += where;
        }
    }
}
