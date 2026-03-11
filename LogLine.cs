using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FountainOfObjects
{
    public class LogLine
    {
        public string Text { get; set; }
        public ConsoleColor Color { get; set; }
        public LogLine(string text, ConsoleColor color)
        {
            Text = text;
            Color = color;
        }
        public void Print()
        {
            Console.ForegroundColor = Color;
            Console.Write(Text);
        }
    }
}
