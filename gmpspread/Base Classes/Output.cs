using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gmpspread.Base_Classes
{
    public static class Output
    {
        public static void Print(string msg) => Console.WriteLine(msg);

        public static ConsoleKeyInfo ReadKey() => Console.ReadKey(true);
    }
}
