using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using VerifyCS = CleanCodeAnalyzer.Test.CSharpAnalyzerVerifier<CleanCodeAnalyzer.CleanCodeAnalyzer>;

namespace CleanCodeAnalyzer.Test
{
    [TestClass]
    public class BlankLineBelowBlockRuleTests
    {
        [TestMethod]
        public async Task TestIfWithOneBlankLineBelow_NoDiagnostic()
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

            Console.WriteLine(""done"");
        }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestIfWithNoBlankLineBelow_ReportsDiagnostic()
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
            Console.WriteLine(""done"");
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0005")
                .WithSpan(15, 13, 15, 14);

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestIfWithTwoBlankLinesBelow_ReportsDiagnostic()
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


            Console.WriteLine(""done"");
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0005")
                .WithSpan(15, 13, 15, 14);

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestLastIfInBlock_NoDiagnostic()
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
        public async Task TestIfElse_NoDiagnostic()
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
            else
            {
                Console.WriteLine(""5 or less"");
            }

            Console.WriteLine(""done"");
        }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestSwitchWithOneBlankLineBelow_NoDiagnostic()
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
                case 1:
                    Console.WriteLine(""one"");
                    break;
                case 2:
                    Console.WriteLine(""two"");
                    break;
            }

            Console.WriteLine(""done"");
        }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestSwitchWithNoBlankLineBelow_ReportsDiagnostic()
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
                case 1:
                    Console.WriteLine(""one"");
                    break;
                case 2:
                    Console.WriteLine(""two"");
                    break;
            }
            Console.WriteLine(""done"");
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0005")
                .WithSpan(20, 13, 20, 14);

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

                Console.WriteLine(""outer"");
            }

            Console.WriteLine(""done"");
        }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestMultipleIfStatements_WithMissingBlankLine_ReportsDiagnostic()
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
            // Only the first if should report diagnostic (followed by another if)
            // The second if should NOT report diagnostic (followed by closing brace)
            var expected = VerifyCS.Diagnostic("CC0005")
                .WithSpan(15, 13, 15, 14);

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestIfBeforeClosingBrace_NoDiagnostic()
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
                if (x > 3)
                {
                    Console.WriteLine(""greater than 3"");
                }
            }
        }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestIfDirectlyBeforeClosingBrace_NoDiagnostic()
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
            // CC0004 should NOT report because the previous statement is a control flow block
            var expected = VerifyCS.Diagnostic("CC0005")
                .WithSpan(15, 13, 15, 14);

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestSwitchBeforeClosingBrace_NoDiagnostic()
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
                case 1:
                    Console.WriteLine(""one"");
                    break;
            }
        }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestMultipleBlocksWithClosingBrace_OnlyFirstReportsDiagnostic()
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
                Console.WriteLine(""first"");
            }
            if (x > 1)
            {
                Console.WriteLine(""second"");
            }
            if (x > 2)
            {
                Console.WriteLine(""third"");
            }
        }
    }
}";
            // Only CC0005 should report (first two ifs need blank line below)
            // CC0004 should NOT report because previous statements are control flow blocks
            var expected1 = VerifyCS.Diagnostic("CC0005")
                .WithSpan(15, 13, 15, 14);
            var expected2 = VerifyCS.Diagnostic("CC0005")
                .WithSpan(19, 13, 19, 14);

            await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2);
        }
    }
}