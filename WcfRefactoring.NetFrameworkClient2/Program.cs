using System;

namespace WcfRefactoring.NetFrameworkClient2
{
    class Program
    {
        static void Main(string[] args)
        {
            var calc = Factory.Create<ICalc>();
            Console.WriteLine(calc.Sum(45, 5));
        }
    }
}
