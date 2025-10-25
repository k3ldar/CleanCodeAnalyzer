using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using VerifyCS = CleanCodeAnalyzer.Test.CSharpAnalyzerVerifier<CleanCodeAnalyzer.CleanCodeAnalyzer>;

namespace CleanCodeAnalyzer.Test
{
    [TestClass]
    public class ChainedAssignmentRuleTests
    {
        [TestMethod]
        public async Task TestSimpleAssignment_NoDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            int a;
            int b;
            int c;
            
            a = 3;
            b = 3;
            c = 3;
        }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestTwoChainedAssignments_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            int a;
            int b;
            
            {|#0:a = b = 5;|}
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0002")
                .WithLocation(0)
                .WithMessage("Chained assignments (e.g., 'a = b = c = 3') should be avoided for clarity");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestThreeChainedAssignments_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            int a;
            int b;
            int c;
            
            {|#0:a = b = c = 3;|}
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0002")
                .WithLocation(0)
                .WithMessage("Chained assignments (e.g., 'a = b = c = 3') should be avoided for clarity");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestMultipleChainedAssignments_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            int a, b, c, d;
            
            {|#0:a = b = c = d = 10;|}
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0002")
                .WithLocation(0)
                .WithMessage("Chained assignments (e.g., 'a = b = c = 3') should be avoided for clarity");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestChainedAssignmentWithExpression_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            int a;
            int b;
            
            {|#0:a = b = 5 + 10;|}
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0002")
                .WithLocation(0)
                .WithMessage("Chained assignments (e.g., 'a = b = c = 3') should be avoided for clarity");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestChainedAssignmentWithMethodCall_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        int GetValue() => 42;

        void TestMethod()
        {
            int x;
            int y;
            
            {|#0:x = y = GetValue();|}
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0002")
                .WithLocation(0)
                .WithMessage("Chained assignments (e.g., 'a = b = c = 3') should be avoided for clarity");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestChainedAssignmentInFieldInitializer_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        private int x;
        private int y;

        public TestClass()
        {
            {|#0:x = y = 100;|}
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0002")
                .WithLocation(0)
                .WithMessage("Chained assignments (e.g., 'a = b = c = 3') should be avoided for clarity");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestSeparateAssignmentsOnDifferentLines_NoDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            int a;
            int b;
            int c;
            
            c = 3;
            b = c;
            a = b;
        }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestCompoundAssignment_NoDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            int a = 5;
            a += 10;
            a -= 3;
            a *= 2;
            a /= 4;
        }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestChainedAssignmentWithProperties_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        public int Value1 { get; set; }
        public int Value2 { get; set; }

        void TestMethod()
        {
            {|#0:Value1 = Value2 = 50;|}
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0002")
                .WithLocation(0)
                .WithMessage("Chained assignments (e.g., 'a = b = c = 3') should be avoided for clarity");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestMultipleIndependentChainedAssignments_ReportsTwoDiagnostics()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            int a, b, c;
            int x, y, z;
            
            {|#0:a = b = 10;|}
            {|#1:x = y = 20;|}
        }
    }
}";
            var expected1 = VerifyCS.Diagnostic("CC0002")
                .WithLocation(0)
                .WithMessage("Chained assignments (e.g., 'a = b = c = 3') should be avoided for clarity");

            var expected2 = VerifyCS.Diagnostic("CC0002")
                .WithLocation(1)
                .WithMessage("Chained assignments (e.g., 'a = b = c = 3') should be avoided for clarity");

            await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2);
        }

        [TestMethod]
        public async Task TestAssignmentWithParentheses_NoDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            int a;
            int b = 5;
            
            a = (b + 10);
        }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestChainedAssignmentWithStringType_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            string s1;
            string s2;
            
            {|#0:s1 = s2 = ""Hello"";|}
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0002")
                .WithLocation(0)
                .WithMessage("Chained assignments (e.g., 'a = b = c = 3') should be avoided for clarity");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}