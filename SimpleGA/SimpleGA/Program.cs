using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGA
{
    class Program
    {
        static void Main(string[] args)
        {
            GA ga = new GA("to be or not to be", 150, 0.01);
            ga.Evolution();
            Console.Read();
        }
    }
}