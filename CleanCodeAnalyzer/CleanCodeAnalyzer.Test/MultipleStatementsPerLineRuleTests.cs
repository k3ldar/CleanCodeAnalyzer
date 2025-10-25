using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using VerifyCS = CleanCodeAnalyzer.Test.CSharpAnalyzerVerifier<CleanCodeAnalyzer.CleanCodeAnalyzer>;

namespace CleanCodeAnalyzer.Test
{
    [TestClass]
    public class MultipleStatementsPerLineRuleTests
    {
        [TestMethod]
        public async Task TestNoStatementsOnSameLine_NoDiagnostic()
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
            int y = 10;
            Console.WriteLine(x + y);
        }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestTwoStatementsOnSameLine_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            int x = 5; {|#0:int y = 10;|}
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0001")
                .WithLocation(0)
                .WithMessage("Multiple statements should not be placed on the same line");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestThreeStatementsOnSameLine_ReportsTwoDiagnostics()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            int x = 5; {|#0:int y = 10;|} {|#1:int z = 15;|}
        }
    }
}";
            var expected1 = VerifyCS.Diagnostic("CC0001")
                .WithLocation(0)
                .WithMessage("Multiple statements should not be placed on the same line");

            var expected2 = VerifyCS.Diagnostic("CC0001")
                .WithLocation(1)
                .WithMessage("Multiple statements should not be placed on the same line");

            await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2);
        }

        [TestMethod]
        public async Task TestMultipleLinesWithMultipleStatements_ReportsDiagnostics()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            int a = 1; {|#0:int b = 2;|}
            int c = 3;
            int d = 4; {|#1:int e = 5;|}
        }
    }
}";
            var expected1 = VerifyCS.Diagnostic("CC0001")
                .WithLocation(0)
                .WithMessage("Multiple statements should not be placed on the same line");

            var expected2 = VerifyCS.Diagnostic("CC0001")
                .WithLocation(1)
                .WithMessage("Multiple statements should not be placed on the same line");

            await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2);
        }

        [TestMethod]
        public async Task TestSwitchSectionStatements_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod(int value)
        {
            switch (value)
            {
                case 1:
                    int x = 1; {|#0:int y = 2;|}
                    break;
                case 2:
                    int z = 3;
                    break;
            }
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0001")
                .WithLocation(0)
                .WithMessage("Multiple statements should not be placed on the same line");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestNestedBlocks_ReportsDiagnostic()
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
                int x = 1; {|#0:int y = 2;|}
            }
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0001")
                .WithLocation(0)
                .WithMessage("Multiple statements should not be placed on the same line");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestEmptyMethod_NoDiagnostic()
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
        public async Task TestSingleStatement_NoDiagnostic()
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
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestForLoopStatements_ReportsDiagnostic()
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
                int x = i; {|#0:int y = i * 2;|}
            }
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0001")
                .WithLocation(0)
                .WithMessage("Multiple statements should not be placed on the same line");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestWhileLoopStatements_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            int counter = 0;
            while (counter < 10)
            {
                counter++; {|#0:int temp = counter;|}
            }
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0001")
                .WithLocation(0)
                .WithMessage("Multiple statements should not be placed on the same line");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestMultipleMethodsWithIssues_ReportsDiagnostics()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void Method1()
        {
            int x = 1; {|#0:int y = 2;|}
        }

        void Method2()
        {
            int a = 10; {|#1:int b = 20;|}
        }
    }
}";
            var expected1 = VerifyCS.Diagnostic("CC0001")
                .WithLocation(0)
                .WithMessage("Multiple statements should not be placed on the same line");

            var expected2 = VerifyCS.Diagnostic("CC0001")
                .WithLocation(1)
                .WithMessage("Multiple statements should not be placed on the same line");

            await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2);
        }
    }
}