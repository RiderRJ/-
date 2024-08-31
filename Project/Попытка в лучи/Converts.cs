using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Попытка_в_лучи
{
    internal class Converts
    {
        private const float angToPie = 1 / 3.14f * 180f;
        public static float ToRadians(float angAngle)
        {
            return angAngle / angToPie;
        }
        public static float ToAngular(float radAngle)
        {
            return radAngle * angToPie;
        }

    }
}
