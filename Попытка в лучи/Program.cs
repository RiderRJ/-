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
    public struct Vector2
    {
        //точки начала
        public float x1;
        public float y1;
        //точки конца
        public float x;
        public float y;
        public Vector2(float x, float y)
        {
            x1 = 0f;
            y1 = 0f;
            this.x = x; this.y = y;
        }
        public Vector2(float x1, float y1, float x2, float y2)
        {
            this.x1 = x1; this.y1 = y1;
            x = x2; y = y2;
        }
        public float Magnitude
        {
            get
            {
                return (float)Math.Sqrt((x - x1) * (x - x1) + (y - y1) * (y - y1));
            }
        }
        public float Distance(Vector2 target)
        {
            return new Vector2(x, y, target.x, target.y).Magnitude;
        }
        /// <summary>
        /// Возвращает угол в радианах относительно вызвавшего вектора, как центра координат
        /// </summary>
        /// <param name="between"></param>
        /// <returns></returns>
        public float SubjectiveRotation(Vector2 between)
        {
            float y = this.y - between.y;
            float x = this.x - between.x;
            float angle = (float)Math.Atan2(y, x);
            if (y == 0 || x == 0) return 180f;
            return angle < 0 ? angle + 2 * 3.14f : angle;
        }
        public Vector2 Normalized()
        {
            return this / Magnitude;
        }
        public Vector2 Invert()
        {
            return new Vector2(x, y, x1, y1);
        }
        public void Invert(ref Vector2 origin)
        {
            origin = new Vector2(origin.x,origin.y,origin.x1,origin.y1);
        }

        public Vector2 Rotate(float angle)
        {
            float ca = (float)Math.Cos(Converts.ToRadians(angle));
            float sa = (float)Math.Sin(Converts.ToRadians(angle));
            return new Vector2(ca * x - sa *y, sa * x + ca * y);
        }
        public float Rotation
        {
            get
            {
                float centerizedY = y - y1;
                float centerizedX = x - x1;
                float angle = (float)Math.Atan2(centerizedY, centerizedX);
                return angle < 0 ? angle + 2 * 3.14f : angle;
            }  
        }
        public float AngularRotation
        {
            get
            {
                return Converts.ToAngular(Rotation);
            }
        }
        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return (a.x == b.x) && (a.x1 == b.x1) && (a.y == b.y) && (a.y1 == b.y1);
        }
        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return (a.x != b.x) && (a.x1 != b.x1) && (a.y != b.y) && (a.y1 != b.y1);
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x1, a.y1, a.x + b.x-b.x1, a.y + b.y - b.y1);
        }
        public static Vector2 operator *(Vector2 a, float b)
        {
            return new Vector2(a.x1, a.y1, (a.x - a.x1) * b + a.x1, (a.y - a.y1) * b + a.y1);//
        }
        public static Vector2 operator /(Vector2 a, float b)
        {
            return new Vector2(a.x / b, a.y / b);
        }
        public override string ToString()
        {
            return $"x1={x1} y1={y1} - x2={x} y2={y}";
        }
    }
    public struct Rectangle
    {
        public Rectangle(Vector2 x, Vector2 y)
        {
            if (x.y1 > 0 || x.x1 > 0 || y.x1 > 0 || y.y1 > 0) throw new ArgumentException();
            this.x = x; this.y = y;
        }
        public Rectangle Invert()
        {
            return new Rectangle(new Vector2(y.x, x.y), new Vector2(x.x, y.y));
        }
        public float Distance(Vector2 target)
        {
            Rectangle inverted = Invert();
            float a = Math.Min(new Vector2(target.x - target.x1, target.y - target.y1, x.x - target.x1, x.y - target.y1).Magnitude,
                new Vector2(target.x - target.x1, target.y - target.y1, y.x - target.x1, y.y - target.y1).Magnitude);
            float b = Math.Min(new Vector2(target.x - target.x1, target.y - target.y1, inverted.x.x - target.x1, inverted.x.y - target.y1).Magnitude,
                new Vector2(target.x - target.x1, target.y - target.y1, inverted.y.x - target.x1, inverted.y.y - target.y1).Magnitude);
            return Math.Min(a, b);
        }
        public bool Intersects(Rectangle b)
        {
            Rectangle a = this;
            if (b.x.x > a.x.x && b.x.y > a.x.y && b.x.x < a.y.x && b.x.y < a.y.y
                || b.y.x > a.x.x && b.y.y > a.x.y && b.y.x < a.y.x && b.y.y < a.y.y
                || a.x.x > b.x.x && a.x.y > b.x.y && a.x.x < b.y.x && a.x.y < b.y.y
                || a.y.x > b.x.x && a.y.y > b.x.y && a.y.x < b.y.x && a.y.y < b.y.y)
                return true;    
            return false;
        }
        public Vector2 x;
        public Vector2 y;
    }
    public struct Ray
    {
        public Ray(Vector2 starterPoint, float angle)
        {
            if (angle > 360 || angle < 0) this.angle = angle - (int)(angle / 360); //поменять потом
            this.angle = angle;
            this.starterPoint = starterPoint;
        }
        public Vector2 starterPoint;
        public float angle;
    }
    public struct RayCast
    {
        public float stepLength;
        private Rectangle[] localField;
        private float depth;
        public RayCast(float step, float depth,ref Rectangle[] field)
        {
            stepLength = step;
            localField = field;
            this.depth = depth;
        }
        private bool CanHit(Ray ray, Rectangle obj, ref Vector2 hit)
        {
            Rectangle invertedObj = obj.Invert();
            List<Vector2> intesectedSides = new List<Vector2>(); 
            List<float> intesections = new List<float>();
            List<float> distances = new List<float>();
            Vector2[] sides = new Vector2[]
            {
                   new Vector2(obj.x.x,obj.x.y,invertedObj.y.x,invertedObj.y.y),
                   new Vector2(obj.x.x,obj.x.y,invertedObj.x.x,invertedObj.x.y),
                   new Vector2(obj.y.x,obj.y.y,invertedObj.y.x,invertedObj.y.y),
                   new Vector2(obj.y.x,obj.y.y,invertedObj.x.x,invertedObj.x.y),
            };
            foreach (var side in sides)
            {
                Vector2 start = new Vector2(ray.starterPoint.x, ray.starterPoint.y, side.x1, side.y1);
                Vector2 end = new Vector2(ray.starterPoint.x, ray.starterPoint.y, side.x, side.y);
                float x = Converts.ToAngular(start.SubjectiveRotation(ray.starterPoint));
                float y = Converts.ToAngular(end.SubjectiveRotation(ray.starterPoint));
                float xAng = Math.Max(x,y);
                float yAng = Math.Min(x, y); 
                Vector2 rotatedVector = new Vector2(start.x,start.y,end.x,end.y);
                if (x == xAng)
                    rotatedVector.Invert(ref rotatedVector);
                if (ray.angle < xAng && ray.angle > yAng
                    || ray.angle < yAng && ray.angle > xAng)
                {
                    intesections.Add((ray.angle - yAng) / (xAng - yAng));
                    intesectedSides.Add(rotatedVector);
                    distances.Add(Math.Min(ray.starterPoint.Distance(start), ray.starterPoint.Distance(end)));
                }
            }
            if(intesections.Count > 0)
            {
                int drawWallIndex = distances.FindIndex(dot => dot == distances.Min());
                hit = intesectedSides[drawWallIndex] * intesections[drawWallIndex];
                return true;
            }

            //if (ray.angle < xAng && ray.angle > yAng
            //    || ray.angle < yAng && ray.angle > xAng
            //    || ray.angle < xInvAng && ray.angle > yInvAng
            //    || ray.angle < yInvAng && ray.angle > xInvAng)
            //    return true;
            return false;
        }
        //мб проверять CanHit сначала с самых близжайших объектов.
        public async Task<float[]> Hit(Ray ray)
        {
            Vector2 hit = new Vector2();
            bool canHit = false;
            float _depth = depth;
            IEnumerable<Rectangle> field = localField.ToList().Where(obj => obj.Distance(ray.starterPoint) <= _depth);
            foreach (var rect in field) 
                if (CanHit(ray, rect, ref hit))
                {
                    Program.canHit = true;
                    canHit = true;
                    return new float[] { hit.x, hit.y };
                }
            return new float[] { float.NaN, float.NaN };
        }
        public async Task<Vector2[]> HitsAsync(Vector2 start, float startAngle, float endAngle, float angularStep)
        {
            HashSet<Vector2> hits = new HashSet<Vector2>();
            List<Task<float[]>> tasks = new List<Task<float[]>>();
            for (float i = startAngle; i < endAngle; i += angularStep)
            {
                Ray ray = new Ray(start, i);
                tasks.Add(Hit(ray));
            }
            float[][] results = await Task.WhenAll(tasks);
            foreach (var hitPos in results)
            {
                hits.Add(new Vector2(hitPos[0], hitPos[1]));
            }
            hits.RemoveWhere(x => float.IsNaN(x.x) || float.IsNaN(x.y));
            return hits.ToArray();
        }
    }
    internal class Program
    {
        private readonly int width = 20;
        private readonly int height = 40;
        private int time = 0;
        private static int mode = 48;
        private static string modeName;
        private string screen;
        private int frameNumber = 0;
        private int fps;
        //игровая карта
        private static Rectangle[] field;
        public static bool canHit;
        static Camera cam; 
        private float startAngle;
        private float endAngle;

        static void Main(string[] args)
        {
            Program instance = new Program();
            cam = new Camera(new Vector2(10,20),0f,45f, 30f);
            Rectangle a = new Rectangle(new Vector2(3, 3), new Vector2(5, 5));
            Rectangle b = new Rectangle(new Vector2(15, 7), new Vector2(17, 9));
            Rectangle c = new Rectangle(new Vector2(15, 15), new Vector2(17, 17));
            Rectangle d = new Rectangle(new Vector2(7, 7), new Vector2(9, 9));
            field = new[] { a, b, c, d };
            instance.FPSReset();
            instance.Update();
            while(true)
            {
                mode = Console.ReadKey().KeyChar;
                switch(mode)
                {
                    case 48: //0
                        modeName = "mapDisabled, rayCasting on";
                        break; 
                    case 49: //1
                        modeName = "mapEnabled, rayCasting off";
                        break;
                    default:
                        modeName = "mapDisabled, rayCasting off";
                        break;
                }
            }
        }
        private async Task FPSReset()
        {
            while(true)
            {
                await Task.Delay(1000);
                fps = frameNumber;
                frameNumber = 0;
            }
        }
        //Даже если удалить обработку, весь код займёт в секунду 12 кадров.
        protected async Task Update()
        {
            while (true)
            {
                if (time > 6000)
                    Environment.Exit(0); 
                List<Vector2> hits = new List<Vector2>();
                canHit = false;
                time++;
                frameNumber++;
                cam.Rotation -= 15f;
                startAngle = cam.Rotation - cam.fov / 2f;
                endAngle = cam.Rotation + cam.fov / 2f;
                Think(out hits);
                await Draw(hits.ToArray());
                await Task.Delay(100);
            }
        }
        private void Think(out List<Vector2> hits)
        {
            RayCast rayCast = new RayCast(0.0005f, cam.dof, ref field);
            hits = rayCast.HitsAsync(cam.pos, startAngle, endAngle, 1f).Result.ToList();
        }
        protected async Task Draw(Vector2[] hits)//нужно придумать, как по-другому отрисовывать кадр
        {
            screen = ""; 
            int drawCall = 0;
            await Task.Run(() =>
            {
                for (float i = 0; i < width; i++)
                {
                    for (float j = 0; j < height; j++)
                    {
                        string draw = " ";
                        if (mode == 48)
                            foreach (var elem in hits)
                            {
                                if (new Rectangle(new Vector2(elem.x, elem.y), new Vector2(elem.x, elem.y)).Intersects(new Rectangle(new Vector2(i - 1f, j - 1f), new Vector2(i + 1f, j + 1f))))
                                {
                                    drawCall++;
                                    draw = "#";
                                    break;
                                }
                                FovVisual(startAngle, endAngle, new Vector2(i, j), ref draw);
                            }
                        if (mode == 49)
                        {
                            foreach (var elem in field)
                            {
                                if (elem.Intersects(new Rectangle(new Vector2(i, j), new Vector2(i + 0.25f, j + 0.25f))))
                                {
                                    draw = "#";
                                    drawCall++;
                                    break;
                                }
                                FovVisual(startAngle, endAngle, new Vector2(i, j), ref draw);
                            }
                        }
                        else FovVisual(startAngle, endAngle, new Vector2(i, j), ref draw);

                        screen += draw;
                    }
                    screen += "|\r\n";
                }
            });
            screen += $"Hits = {hits.Length} DrawWallCounts = {drawCall} \r\nCamera rotation = {cam.Rotation} \r\nHit prediction = {canHit}\r\nMode = {modeName}\r\nFps = {fps}";
            Clear();
            Console.Write(screen);
        }
        private void FovVisual(float xAng, float yAng, Vector2 dot, ref string draw)
        {
            if (draw != " ") return;
            Vector2 subjective = new Vector2(cam.pos.x, cam.pos.y, dot.x, dot.y);
            float subjRotation = subjective.AngularRotation;
            if (subjRotation >= xAng && subjRotation <= yAng 
                || subjRotation <= xAng && subjRotation >= yAng)
                draw = "-";
        }
        private void Clear()
        {
            Console.Clear();
        }
    }
}