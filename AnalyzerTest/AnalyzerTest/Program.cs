using System;

namespace AnalyzerTest
{
    class Program
    {
        static int foo()
        {
            return 1;
        }
        static void Main(string[] args)
        {
            int x = 5 * 7 * 9 / 5; //correct one
            int y = 99 / 0; //uncorrect
            int z = foo() / 0; //uncorrect
            int a = 5*0; //correct
            
            Console.WriteLine("Hello World! {0}",5/0); //uncorrect
        }
    }
}
