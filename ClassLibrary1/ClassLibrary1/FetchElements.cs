using System;
namespace CSharpStrings
{
    class Program
    {
        static void Main(string[] args)
        {
            // Define .NET Strings
            // String of characters
            System.String authorName = "Mahesh Chand";

            // String made of an Integer
            System.String age = "33";

            // String made of a double
            System.String numberString = "33.23";

            // Write to Console.

            Console.WriteLine("Name: {0}", authorName);
            Console.WriteLine("Age: {0}", age);
            Console.WriteLine("Number: {0}", numberString);
            Console.ReadKey();

        }
    }
}