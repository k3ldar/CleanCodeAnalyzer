using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using VerifyCS = CleanCodeAnalyzer.Test.CSharpAnalyzerVerifier<CleanCodeAnalyzer.CleanCodeAnalyzer>;

namespace CleanCodeAnalyzer.Test
{
    [TestClass]
    public class NoBlankLineBeforeClosingBraceRuleTests
    {
        [TestMethod]
        public async Task TestMethodWithNoBlankLineBeforeClosingBrace_NoDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            int x = 5;
            Console.WriteLine(x);
        }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestMethodWithBlankLineBeforeClosingBrace_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            int x = 5;
            Console.WriteLine(x);

        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0008")
                .WithSpan(12, 1, 12, 1);

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestSwitchWithBlankLineBeforeClosingBrace_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            int x = 5;

            switch (x)
            {
                case 5:
                    Console.WriteLine(""five"");
                    break;

            }
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0008")
                .WithSpan(17, 1, 17, 1);

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestIfStatementWithBlankLineBeforeClosingBrace_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            int x = 5;

            if (x > 0)
            {
                Console.WriteLine(""positive"");

            }
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0008")
                .WithSpan(15, 1, 15, 1);

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestClassWithBlankLineBeforeClosingBrace_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            int x = 5;
        }

    }
}";
            var expected = VerifyCS.Diagnostic("CC0008")
                .WithSpan(12, 1, 12, 1);

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestNamespaceWithBlankLineBeforeClosingBrace_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            int x = 5;
        }
    }

}";
            var expected = VerifyCS.Diagnostic("CC0008")
                .WithSpan(13, 1, 13, 1);

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestMultipleBlankLinesBeforeClosingBrace_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            int x = 5;
            Console.WriteLine(x);


        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0008")
                .WithSpan(13, 1, 13, 1);

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestNestedBlocksWithBlankLines_ReportsMultipleDiagnostics()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            int x = 5;

            if (x > 0)
            {
                Console.WriteLine(""positive"");

            }

        }
    }
}";
            var expected1 = VerifyCS.Diagnostic("CC0008")
                .WithSpan(15, 1, 15, 1);
            var expected2 = VerifyCS.Diagnostic("CC0008")
                .WithSpan(17, 1, 17, 1);

            await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2);
        }

        [TestMethod]
        public async Task TestForLoopWithBlankLineBeforeClosingBrace_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(i);

            }
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0008")
                .WithSpan(13, 1, 13, 1);

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestWhileLoopWithBlankLineBeforeClosingBrace_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            int x = 0;

            while (x < 10)
            {
                Console.WriteLine(x);
                x++;

            }
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0008")
                .WithSpan(16, 1, 16, 1);

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestTryCatchWithBlankLineBeforeClosingBrace_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            try
            {
                Console.WriteLine(""try"");

            }
            catch (Exception ex)
            {
                Console.WriteLine(""catch"");

            }
        }
    }
}";
            var expected1 = VerifyCS.Diagnostic("CC0008")
                .WithSpan(13, 1, 13, 1);
            var expected2 = VerifyCS.Diagnostic("CC0008")
                .WithSpan(18, 1, 18, 1);

            await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2);
        }

        [TestMethod]
        public async Task TestEmptyBlock_NoDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
        }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestSingleStatementBlock_NoDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            Console.WriteLine(""test"");
        }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestPropertyWithBlankLineBeforeClosingBrace_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        public int MyProperty
        {
            get
            {
                return 5;

            }

            set
            {
                Console.WriteLine(value);

            }
        }
    }
}";
            var expected1 = VerifyCS.Diagnostic("CC0008")
                .WithSpan(13, 1, 13, 1);
            var expected2 = VerifyCS.Diagnostic("CC0008")
                .WithSpan(19, 1, 19, 1);

            await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2);
        }

        [TestMethod]
        public async Task TestLambdaWithBlankLineBeforeClosingBrace_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            Action action = () =>
            {
                Console.WriteLine(""lambda"");

            };
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0008")
                .WithSpan(13, 1, 13, 1);

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}