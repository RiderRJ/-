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
    interface IThinker
    {
        List<Vector2> Vision { get; set; }
        List<Vector2> Other { get; set; }
        void Think();
    }
}
