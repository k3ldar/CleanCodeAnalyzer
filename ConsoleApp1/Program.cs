// See https://aka.ms/new-console-template for more information
using ConsoleApp1;

Console.WriteLine("Hello, World!");

int x = 5; int y = 10; Console.WriteLine(x + y);

TestErrors te = new();
te.Test();
TestMethod();

static void TestMethod()
{
    int x = 5; int y = 10; Console.WriteLine(x + y);

    int a;
    int b;
    int c;

    a = b = c = 3;
}