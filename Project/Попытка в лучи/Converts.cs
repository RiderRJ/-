using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Попытка_в_лучи
{
    internal class Converts
    {
        private const float pie = 3.1415926f;
        //private const float pie = 3.14f;
        private const float angToPie = 1 / pie * 180f;
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
