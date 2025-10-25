using System.Threading.Tasks;

using CleanCodeAnalyzer.Rules;

using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CleanCodeAnalyzer.Test
{
    [TestClass]
    public class MagicNumberAnalyzerTests
    {
        [TestMethod]
        public async Task ReportsMagicNumber()
        {
            var testCode = @"
using System;

class Person
{
    public void Register(int age)
    {
        if (age >= [|18|])
            Console.WriteLine([|""OK""|]);
    }
}";

            var test = new CSharpAnalyzerTest<MagicNumberAnalyzer, MSTestVerifier>
            {
                TestCode = testCode
            };

            await test.RunAsync();
        }

        [TestMethod]
        public async Task DoesNotReportZeroInForInitializer_ButReportsOtherLiteral()
        {
            var testCode = @"
class C
{
    void M()
    {
        for (int i = 0; i < [|10|]; i++)
        {
        }
    }
}";

            var test = new CSharpAnalyzerTest<MagicNumberAnalyzer, MSTestVerifier>
            {
                TestCode = testCode
            };

            await test.RunAsync();
        }

        [TestMethod]
        public async Task SkipsConstDeclaration()
        {
            var testCode = @"
class C
{
    private const int TimeoutMs = 250;
    void M()
    {
        var x = TimeoutMs;
    }
}";

            var test = new CSharpAnalyzerTest<MagicNumberAnalyzer, MSTestVerifier>
            {
                TestCode = testCode
            };

            await test.RunAsync();
        }

        [TestMethod]
        public async Task SkipsLocalConstDeclaration()
        {
            var testCode = @"
class C
{
    void M()
    {
        const int BufferSize = 1024;
        var buffer = new byte[BufferSize];
    }
}";

            var test = new CSharpAnalyzerTest<MagicNumberAnalyzer, MSTestVerifier>
            {
                TestCode = testCode
            };

            await test.RunAsync();
        }

        [TestMethod]
        public async Task AllowsTrivialLiterals_ZeroOneNegativeOne()
        {
            var testCode = @"
class C
{
    void M()
    {
        int a = 0;
        int b = 1;
        int c = -1;
    }
}";

            var test = new CSharpAnalyzerTest<MagicNumberAnalyzer, MSTestVerifier>
            {
                TestCode = testCode
            };

            await test.RunAsync();
        }

        [TestMethod]
        public async Task AllowsBooleanLiterals()
        {
            var testCode = @"
class C
{
    void M()
    {
        bool x = true;
        bool y = false;
    }
}";

            var test = new CSharpAnalyzerTest<MagicNumberAnalyzer, MSTestVerifier>
            {
                TestCode = testCode
            };

            await test.RunAsync();
        }

        [TestMethod]
        public async Task AllowsEmptyStringLiteral()
        {
            var testCode = @"
class C
{
    void M()
    {
        string s = """";
    }
}";

            var test = new CSharpAnalyzerTest<MagicNumberAnalyzer, MSTestVerifier>
            {
                TestCode = testCode
            };

            await test.RunAsync();
        }

        [TestMethod]
        public async Task ReportsNonEmptyStringLiteral()
        {
            var testCode = @"
class C
{
    void M()
    {
        string s = [|""ConnectionString""|];
    }
}";

            var test = new CSharpAnalyzerTest<MagicNumberAnalyzer, MSTestVerifier>
            {
                TestCode = testCode
            };

            await test.RunAsync();
        }

        [TestMethod]
        public async Task ReportsCharLiteral()
        {
            var testCode = @"
class C
{
    void M()
    {
        char delimiter = [|','|];
    }
}";

            var test = new CSharpAnalyzerTest<MagicNumberAnalyzer, MSTestVerifier>
            {
                TestCode = testCode
            };

            await test.RunAsync();
        }

        [TestMethod]
        public async Task SkipsEnumMemberInitializers()
        {
            var testCode = @"
enum Status
{
    Pending = 0,
    Active = 1,
    Cancelled = 99
}";

            var test = new CSharpAnalyzerTest<MagicNumberAnalyzer, MSTestVerifier>
            {
                TestCode = testCode
            };

            await test.RunAsync();
        }

        [TestMethod]
        public async Task SkipsAttributeArguments()
        {
            var testCode = @"
using System;

class C
{
    [Obsolete(""Use NewMethod"", true)]
    void OldMethod() { }
}";

            var test = new CSharpAnalyzerTest<MagicNumberAnalyzer, MSTestVerifier>
            {
                TestCode = testCode
            };

            await test.RunAsync();
        }

        [TestMethod]
        public async Task ReportsMultipleMagicNumbersInSameMethod()
        {
            var testCode = @"
class C
{
    void M()
    {
        int timeout = [|30|];
        int retries = [|3|];
        double percentage = [|0.95|];
    }
}";

            var test = new CSharpAnalyzerTest<MagicNumberAnalyzer, MSTestVerifier>
            {
                TestCode = testCode
            };

            await test.RunAsync();
        }

        [TestMethod]
        public async Task ReportsNegativeMagicNumber()
        {
            var testCode = @"
class C
{
    void M()
    {
        int value = [|-999|];
    }
}";

            var test = new CSharpAnalyzerTest<MagicNumberAnalyzer, MSTestVerifier>
            {
                TestCode = testCode
            };

            await test.RunAsync();
        }

        [TestMethod]
        public async Task ReportsDecimalLiteral()
        {
            var testCode = @"
class C
{
    void M()
    {
        decimal taxRate = [|0.08m|];
    }
}";

            var test = new CSharpAnalyzerTest<MagicNumberAnalyzer, MSTestVerifier>
            {
                TestCode = testCode
            };

            await test.RunAsync();
        }
    }
}