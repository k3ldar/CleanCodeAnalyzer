using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using VerifyCS = CleanCodeAnalyzer.Test.CSharpAnalyzerVerifier<CleanCodeAnalyzer.CleanCodeAnalyzer>;

namespace CleanCodeAnalyzer.Test
{
    [TestClass]
    public class BlankLineAboveIfStatementRuleTests
    {
        [TestMethod]
        public async Task TestIfWithOneBlankLineAbove_NoDiagnostic()
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
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestIfWithNoBlankLineAbove_ReportsDiagnostic()
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
            var expected = VerifyCS.Diagnostic("CC0004")
                .WithSpan(11, 13, 11, 23);

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestIfWithTwoBlankLinesAbove_ReportsDiagnostic()
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
            var expected = VerifyCS.Diagnostic("CC0004")
                .WithSpan(13, 13, 13, 23);

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestFirstIfInBlock_NoDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            if (true)
            {
                Console.WriteLine(""test"");
            }
        }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestElseIf_NoDiagnostic()
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

            if (x > 10)
            {
                Console.WriteLine(""greater than 10"");
            }
            else if (x > 5)
            {
                Console.WriteLine(""greater than 5"");
            }
        }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestMultipleIfStatements_ReportsCorrectly()
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
            if (x < 10)
            {
                Console.WriteLine(""less than 10"");
            }
        }
    }
}";
            // Only CC0005 should report (first if needs blank line below)
            // CC0004 should NOT report because previous statement is control flow block
            var expected = VerifyCS.Diagnostic("CC0005")
                .WithSpan(15, 13, 15, 14);

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestNestedIfStatements_HandlesCorrectly()
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
                int y = 10;

                if (y > 5)
                {
                    Console.WriteLine(""nested"");
                }
            }
        }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestIfAfterClosingBrace_NoDiagnostic()
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

            if (x < 10)
            {
                Console.WriteLine(""less than 10"");
            }
        }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}