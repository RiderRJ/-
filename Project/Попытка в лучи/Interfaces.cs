using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Попытка_в_лучи
{
    interface IMovable
    {
        Vector2 Position { get; set; }
        void Move(Vector2 destignation);
    }
    public abstract class Thinker 
    {
        public static List<Thinker> thinkers = new List<Thinker>();
        public Thinker()
        {
            thinkers.Add(this);
        }
        public abstract List<Vector2> Vision { get; set; }
        public abstract List<Vector2> Other { get; set; }
        public abstract void Think();
    }
}
