using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
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
        public float Angle(Vector2 between)
        {
            return between.Rotation - Rotation;
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
            origin = new Vector2(origin.x, origin.y, origin.x1, origin.y1);
        }

        public Vector2 Rotate(float angleInDegrees)//надо изучить формулу и исправить баг с поворотом вектора
        {
            float ca = (float)Math.Cos(Converts.ToRadians(angleInDegrees));
            float sa = (float)Math.Sin(Converts.ToRadians(angleInDegrees));
            return new Vector2(x1, y1, ca * (x - x1) - sa * (y - y1) + x1, sa * (x - x1) + ca * (y - y1) + y1);
            //return new Vector2(ca * (x - x1) + x1, sa * (y - y1) + y1); //сравнить обе версии, прийти к общей
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
            return new Vector2(a.x1, a.y1, a.x + b.x - b.x1, a.y + b.y - b.y1);
        }
        public static Vector2 operator *(Vector2 a, float b)
        {
            return new Vector2(a.x1, a.y1, (a.x - a.x1) * b + a.x1, (a.y - a.y1) * b + a.y1);//
        }
        public static float operator *(Vector2 a, Vector2 b)
        {
            return (float)Math.Cos(a.Angle(b)) * a.Magnitude * b.Magnitude;
        }
        public static Vector2 operator /(Vector2 a, float b)
        {
            //return new Vector2(a.x / b, a.y / b);
            return new Vector2(a.x1, a.y1, (a.x - a.x1) / b + a.x1, (a.y - a.y1) / b + a.y1);
        }
        public override string ToString()
        {
            return $"x1={x1} y1={y1} - x2={x} y2={y}";
        }

        //public override bool Equals(object obj)
        //{
        //    return obj is Vector2 vector &&
        //           x1 == vector.x1 &&
        //           y1 == vector.y1 &&
        //           x == vector.x &&
        //           y == vector.y;
        //}

        //public override int GetHashCode()
        //{
        //    int hashCode = 343804948;
        //    hashCode = hashCode * -1521134295 + x1.GetHashCode();
        //    hashCode = hashCode * -1521134295 + y1.GetHashCode();
        //    hashCode = hashCode * -1521134295 + x.GetHashCode();
        //    hashCode = hashCode * -1521134295 + y.GetHashCode();
        //    return hashCode;
        //}
    }
    public struct Rectangle
    {
        public Rectangle(Vector2 x, Vector2 y)
        {
            if (x.y1 > 0 || x.x1 > 0 || y.x1 > 0 || y.y1 > 0) throw new ArgumentException();
            this.x = x; this.y = y;
        }
        public Rectangle(Vector2 x)
        {
            this.x = new Vector2(x.x1,x.y1);
            y = new Vector2(x.x, x.y);
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
            if (x.x > b.y.x || b.x.x > y.x) return false;
            if (x.y > b.y.y || b.x.y > y.y) return false; 
            return true;
        }
        public Rectangle Round(int digits = 0)
        {
            return new Rectangle(new Vector2((float)Math.Round(x.x, digits), (float)Math.Round(x.y, digits)),
                new Vector2((float)Math.Round(y.x), (float)Math.Round(y.y)));
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
        public RayCast(float step, float depth, ref Rectangle[] field)
        {
            stepLength = step;
            localField = field;
            this.depth = depth;
        }
        public async Task<Vector2> Hit(Ray ray)
        {
            float _depth = depth;
            List<Rectangle> field = localField.Where(obj => obj.Distance(ray.starterPoint) <= _depth).ToList();
            field.Sort(delegate (Rectangle x, Rectangle y)
            {
                if (x.Distance(ray.starterPoint) > y.Distance(ray.starterPoint)) return 1;
                if (x.Distance(ray.starterPoint) < y.Distance(ray.starterPoint)) return -1;
                return 0;
            });
            List<float> distances = new List<float>();
            //List<Rectangle> sortedField = new List<Rectangle>();
            List<Vector2> potentialHits = new List<Vector2>();
            List<float> hitsDistances = new List<float>();
            int hitIndex;
            //int sortIterations = field.Count;//мб убрать к х-ям
            //foreach (var item in field)
            //    distances.Add(item.Distance(ray.starterPoint));
            //for (int i = 0; i < sortIterations; i++)
            //{
            //    int minId = distances.FindIndex(distance => distance == distances.Min());
            //    sortedField.Add(field[minId]);
            //    field.RemoveAt(minId);
            //    distances.RemoveAt(minId);
            //}
            foreach (var rect in field)
            {
                Rectangle invertedObj = rect.Invert();
                List<Vector2> intesectedSides = new List<Vector2>();
                List<float> intesections = new List<float>();
                List<float> intersectDistances = new List<float>();
                Vector2[] sides = new Vector2[]
                {
                   new Vector2(rect.x.x,rect.x.y,invertedObj.y.x,invertedObj.y.y),
                   new Vector2(rect.x.x,rect.x.y,invertedObj.x.x,invertedObj.x.y),
                   new Vector2(rect.y.x,rect.y.y,invertedObj.y.x,invertedObj.y.y),
                   new Vector2(rect.y.x,rect.y.y,invertedObj.x.x,invertedObj.x.y),
                };
                foreach (var side in sides)
                {
                    Vector2 start = new Vector2(ray.starterPoint.x, ray.starterPoint.y, side.x1, side.y1);
                    Vector2 end = new Vector2(ray.starterPoint.x, ray.starterPoint.y, side.x, side.y);
                    float x = Converts.ToAngular(start.SubjectiveRotation(ray.starterPoint));
                    float y = Converts.ToAngular(end.SubjectiveRotation(ray.starterPoint));
                    float maxLim = Math.Max(x, y);
                    float minLim = Math.Min(x, y);
                    Vector2 rotatedVector = new Vector2(start.x, start.y, end.x, end.y);
                    if (x == maxLim)
                        rotatedVector.Invert(ref rotatedVector);
                    if (ray.angle < maxLim && ray.angle > minLim
                        || ray.angle < minLim && ray.angle > maxLim)
                    {
                        intesections.Add((ray.angle - minLim) / (maxLim - minLim));
                        intesectedSides.Add(rotatedVector);
                        intersectDistances.Add(Math.Min(ray.starterPoint.Distance(start), ray.starterPoint.Distance(end)));
                    }
                }
                if (intesections.Count > 0)
                {
                    Program.canHit = true;
                    int drawWallIndex = intersectDistances.FindIndex(dot => dot == intersectDistances.Min());
                    potentialHits.Add(intesectedSides[drawWallIndex] * intesections[drawWallIndex]);
                    hitsDistances.Add(ray.starterPoint.Distance(potentialHits.Last()));
                }
            }
            if (potentialHits.Count == 0) return new Vector2(float.NaN, float.NaN);
            hitIndex = hitsDistances.FindIndex(item => item == hitsDistances.Min());
            return potentialHits[hitIndex];
        }
        public async Task<Vector2[]> HitsAsync(Vector2 start, float startAngle, float endAngle, float angularStep) //вместо Async надо сделать многопоточность.
        {
            HashSet<Vector2> hits = new HashSet<Vector2>();
            List<Task<Vector2>> tasks = new List<Task<Vector2>>();
            for (float i = startAngle; i < endAngle; i += angularStep)
            {
                Ray ray = new Ray(start, i);
                tasks.Add(Hit(ray));
            }
            Vector2[] results = await Task.WhenAll(tasks);
            foreach (var hitPos in results)
            {
                hits.Add(hitPos);
            }
            hits.RemoveWhere(x => float.IsNaN(x.x) || float.IsNaN(x.y));
            return hits.ToArray();
        }
    }
    internal class Program
    {
        private readonly int width = 20;
        private readonly int height = 40;
        private static int mode = 48;
        private static float horizon = 0;
        private static int projHeight = 5;//макс высота объекта на расстоянии 1 ~метра
        private static string modeName;
        private string screen;
        private int time = 0;
        private int frameNumber = 0;
        private int fps;
        public static Rectangle[] field;
        public static bool canHit;
        static Camera cam;
        public static List<Vector2> hits = new List<Vector2>();
        public static List<Vector2> collisionHits = new List<Vector2>();
        static void Main(string[] args)
        {
            Program instance = new Program();
            instance.Init();
            instance.FPSReset();
            instance.Update();
            Thread inputCather = new Thread(Controller.StartTracking);
            inputCather.Start();
            while (true)
            {
                switch (mode)
                {
                    case 48: //0
                        modeName = "mapDisabled, rayCasting on";
                        break;
                    case 49: //1
                        modeName = "mapEnabled, rayCasting off";
                        break;
                    case 51: //1
                        modeName = "mapDisabled, rayCastingProjection on";
                        break;
                    default:
                        modeName = "mapDisabled, rayCasting off";
                        break;
                }
                var key = Console.ReadKey();
                if (key.KeyChar == 48 || key.KeyChar == 49 || key.KeyChar == 51)
                    mode = key.KeyChar;
            }
        }
        private async Task FPSReset()
        {
            while (true)
            {
                await Task.Delay(1000);
                fps = frameNumber;
                frameNumber = 0;
            }
        }
        protected async Task Update()
        {
            while (true)
            {
                if (time > 60)
                    Environment.Exit(0);
                canHit = false;
                time++;
                frameNumber++;
                foreach (var thinker in Thinker.thinkers)
                    thinker.Think();
                await Draw(hits.ToArray());
                await Task.Delay(250);
            }
        }
        private void Init()
        {
            //Clear();
            cam = new Camera(new Vector2(10, 20, 10, 20), 270f, 45f, 30f);
            Rectangle a = new Rectangle(new Vector2(3, 3), new Vector2(5, 5));
            Rectangle b = new Rectangle(new Vector2(15, 7), new Vector2(17, 9));
            Rectangle c = new Rectangle(new Vector2(15, 15), new Vector2(17, 17));
            Rectangle d = new Rectangle(new Vector2(7, 7), new Vector2(9, 9));
            Rectangle b1 = new Rectangle(new Vector2(-1, -1), new Vector2(1, 40));//крыша
            Rectangle b2 = new Rectangle(new Vector2(-1, -1), new Vector2(20, 1));//левая
            Rectangle b3 = new Rectangle(new Vector2(19, 39), new Vector2(19, -3));//БАГИИИИ БАГИИИ 
            Rectangle b4 = new Rectangle(new Vector2(18, 18), new Vector2(0, 18));//
            field = new[] { a, b, c, d/*, b1, b2, b3, b4 */};

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
                                if (new Rectangle(new Vector2(elem.x1, elem.y1, elem.x, elem.y)).Round(1).
                                Intersects(new Rectangle(new Vector2(i - 1 / (float)width, j - 1 / (float)height), new Vector2(i + 1 / (float)width, j + 1 / (float)height))))
                                {
                                    drawCall++;
                                    draw = "#";
                                    break;
                                }
                                FovVisual(cam.StartAngle, cam.EndAngle, new Vector2(i, j), ref draw);
                            }
                        if (mode == 49)
                        {
                            foreach (var elem in field)
                            {
                                if (i == 19 && j == 1)
                                    ;
                                if (elem.Round(1).Intersects(new Rectangle(new Vector2(i - 1/(float)width, j - 1 / (float)height), new Vector2(i + 1 / (float)width, j + 1 / (float)height))))
                                {
                                    draw = "#";
                                    drawCall++;
                                    break;
                                }
                                FovVisual(cam.StartAngle, cam.EndAngle, new Vector2(i, j), ref draw);
                            }
                        }
                        if (mode == 51)
                        {
                            //отрисовка полосок
                            foreach (var elem in hits)
                            {
                                float lineLength = projHeight - (projHeight / cam.Position.Distance(elem) / 1 - projHeight);
                                //actualSize - (actualSize / Distance / Const - actualSize);
                            }
                        }
                        else
                            FovVisual(cam.StartAngle, cam.EndAngle, new Vector2(i, j), ref draw);

                        screen += draw;
                    }
                    screen += "|\r\n";
                }
            });
            //screen += $"Hits = {hits.Length} DrawWallCounts = {drawCall} \r\nCamera rotation = {cam.Rotation} \r\nHit prediction = {canHit}\r\nMode = {modeName}\r\nFps = {fps}";
            screen += $"\r\n{cam.Position} Hits = {hits.Length} DrawWallCounts = {drawCall} \r\nCamera rotation = {cam.Rotation} \r\nOther hits = {collisionHits.Count}\r\n" +
                $"Mode = {modeName}\r\nFps = {fps}\r\nCores = {Environment.ProcessorCount} KeyPressed = {Controller.pressedKey}";
            Clear();
            Console.Write(screen);
        }
        private void FovVisual(float xAng, float yAng, Vector2 dot, ref string draw)
        {
            if (draw != " ") return;
            Vector2 subjective = new Vector2(cam.Position.x, cam.Position.y, dot.x, dot.y);
            float subjRotation = subjective.AngularRotation;
            if (subjRotation >= xAng && subjRotation <= yAng
                || subjRotation <= xAng && subjRotation >= yAng)
                draw = "-";
        }
        private void Clear()
        {
            //for (int i = 0; i < width; i++)
            //{
            //    for (int j = 0; j < height; j++)
            //    {
            //        screen += " ";
            //    }
            //    screen += "|\r\n";
            //}
            Console.Clear();
        }
    }
}