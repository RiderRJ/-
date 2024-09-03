using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Попытка_в_лучи
{
    interface IMovable
    {
        void Move();
    }
    interface IThinker
    {
        List<Vector2> vision { get; set; }
        List<Vector2> other { get; set; }
        void Think();
    }
}
