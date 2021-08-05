using System;
using System.Collections.Generic;
using System.Text;

namespace Solitaire
{
    public static class ConsoleMessages
    {
        public static void DisplayMessage(string message, bool newLine, ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black, bool appendEmptySpaceToEnd = true)
        {
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            if (newLine)
            {
                Console.WriteLine(message);
            }
            else
            {
                Console.Write(message);
            }
            Console.ResetColor();
            if (appendEmptySpaceToEnd)
            {
                Console.Write(" ");
            }
        }
    }
}