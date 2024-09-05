using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Попытка_в_лучи
{
    static class Controller
    {
        public enum Key
        {
            W,A,S,D,LeftArrow,RightArrow, NotAKey
        };
        public static Key pressedKey = Key.NotAKey;
        public static void StartTracking()
        {
            _ = Input();
        }
        public static async Task Input()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    var keyCode = Console.ReadKey();
                    var key = keyCode.KeyChar;
                    if (Console.KeyAvailable)
                    {
                        if (key == 'W' || key == 'Ц' || key == 'w' || key == 'ц')
                            pressedKey = Key.W;
                        else
                    if (key == 'A' || key == 'Ф' || key == 'a' || key == 'ф')
                            pressedKey = Key.A;
                        else
                    if (key == 'S' || key == 'Ы' || key == 's' || key == 'ы')
                            pressedKey = Key.S;
                        else
                    if (key == 'D' || key == 'В' || key == 'd' || key == 'в')
                            pressedKey = Key.D;
                        if(keyCode.Key.ToString() == "LeftArrow")
                            pressedKey = Key.LeftArrow;
                        if(keyCode.Key.ToString() == "RightArrow")
                            pressedKey = Key.RightArrow;

                    }
                    //else pressedKey = Key.NotAKey;
                }
            });
        }
        public static bool GetInput(Key keyChar)
        {
            if (keyChar == pressedKey) { return true; }
            return false;
        }
    }
}
