using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Попытка_в_лучи
{
    /// <summary>
    /// Типо при написании класса, я, определяя его наследственность решаю в какой список при старте он будет добавлятся. Так же имею право полностью убрать у него возможность
    /// Реализовываться через списки
    /// Разве что я не могу разрешить объекту добавляться в два списка в качестве Movable и Thinker одновременно. Это придётся делать вручную через код самого объекта.
    /// Хуйня какая-то 
    /// </summary>
    public interface IMovable
    {
        Vector2 Position { get; set; }
        void Move(Vector2 destignation);
    }
    public interface IThinker
    {
        List<Vector2> Vision { get; set; }
        List<Vector2> Other { get; set; }
        void Think();
    }
    public abstract class Movable : IMovable
    {
        public static List<IMovable> movables = new List<IMovable>();
        public abstract Vector2 Position { get; set; }
        public abstract void Move(Vector2 destignation);
        public Movable()
        {
            movables.Add(this);
        }
    }
    public abstract class Thinker : IThinker
    {
        public static List<IThinker> thinkers = new List<IThinker>();
        public abstract List<Vector2> Vision { get; set; }
        public abstract List<Vector2> Other { get; set; }
        public abstract void Think();

        public Thinker()
        {
            thinkers.Add(this);
        }
    }
}
