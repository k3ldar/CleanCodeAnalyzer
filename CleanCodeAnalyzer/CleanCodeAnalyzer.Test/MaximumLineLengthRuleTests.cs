using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using VerifyCS = CleanCodeAnalyzer.Test.CSharpAnalyzerVerifier<CleanCodeAnalyzer.CleanCodeAnalyzer>;

namespace CleanCodeAnalyzer.Test
{
    [TestClass]
    public class MaximumLineLengthRuleTests
    {
        [TestMethod]
        public async Task TestShortLines_NoDiagnostic()
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
        public async Task TestExactly120Characters_NoDiagnostic()
        {
            // Line with exactly 120 characters (should not trigger diagnostic)
            var test = @"
namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            // This line is exactly 120 characters long including spaces and indentation marks here now
        }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestLineTooLong_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            var result = ""This is a very long line that exceeds the maximum line length of 120 characters and should trigger a diagnostic warning"";
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0003")
                .WithSpan(10, 1, 10, 148)
                .WithArguments("147", "120");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestMultipleLongLines_ReportsMultipleDiagnostics()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            var firstLongLine = ""This is a very long line that exceeds the maximum line length of 120 characters and should trigger a diagnostic"";
            int normalLine = 5;
            var secondLongLine = ""This is another very long line that also exceeds the maximum line length of 120 characters and should trigger another diagnostic"";
        }
    }
}";
            var expected1 = VerifyCS.Diagnostic("CC0003")
                .WithSpan(10, 1, 10, 147)
                .WithArguments("146", "120");

            var expected2 = VerifyCS.Diagnostic("CC0003")
                .WithSpan(12, 1, 12, 165)
                .WithArguments("164", "120");

            await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2);
        }

        [TestMethod]
        public async Task TestLongMethodSignature_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void VeryLongMethodNameWithManyParameters(string parameter1, string parameter2, string parameter3, string parameter4, int parameter5, bool parameter6)
        {
            Console.WriteLine(""Test"");
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0003")
                .WithSpan(8, 1, 8, 159)
                .WithArguments("158", "120");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestLongComment_NoDiagnostic()
        {
            // Comments are intentionally excluded from line length checks
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            // This is a very long comment that exceeds the maximum line length of 120 characters and should NOT trigger a diagnostic warning because comments are excluded
            int x = 5;
        }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestLongStringLiteral_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            string longText = ""This is an extremely long string literal that definitely exceeds the maximum allowed line length of 120 characters"";
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0003")
                .WithSpan(10, 1, 10, 148)
                .WithArguments("147", "120");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestEmptyFile_NoDiagnostic()
        {
            var test = @"";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestSingleLineFile_NoDiagnostic()
        {
            var test = @"using System;";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestLongUsingDirective_ReportsDiagnostic()
        {
            var test = @"using SystemAlias = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<int, string>>>;

namespace TestNamespace
{
    class TestClass
    {
    }
}";
            var expected = VerifyCS.Diagnostic("CC0003")
                .WithSpan(1, 1, 1, 154)
                .WithArguments("153", "120");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestLongLinqQuery_ReportsDiagnostic()
        {
            var test = @"
using System.Linq;

namespace TestNamespace
{
    class TestClass
    {
        void TestMethod()
        {
            var numbers = new[] { 1, 2, 3, 4, 5 };
            var result = numbers.Where(n => n > 0).Select(n => n * 2).Where(n => n > 5).OrderBy(n => n).ThenBy(n => n * 3).ToList();
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0003")
                .WithSpan(11, 1, 11, 133)
                .WithArguments("132", "120");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestLongAttributeLine_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        [Obsolete(""This method is deprecated and should not be used anymore because it has been replaced by a better implementation"")]
        void TestMethod()
        {
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0003")
                .WithSpan(8, 1, 8, 135)
                .WithArguments("134", "120");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestMultipleClassesWithLongLines_ReportsMultipleDiagnostics()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class FirstClass
    {
        void FirstMethodWithVeryLongNameAndManyParameters(string param1, string param2, string param3, string param4, string param5)
        {
        }
    }

    class SecondClass
    {
        void SecondMethodWithVeryLongNameAndManyParameters(string param1, string param2, string param3, string param4, string param5)
        {
        }
    }
}";
            var expected1 = VerifyCS.Diagnostic("CC0003")
                .WithSpan(8, 1, 8, 133)
                .WithArguments("132", "120");

            var expected2 = VerifyCS.Diagnostic("CC0003")
                .WithSpan(15, 1, 15, 134)
                .WithArguments("133", "120");

            await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2);
        }

        [TestMethod]
        public async Task TestLongPropertyDeclaration_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        public string VeryLongPropertyNameThatExceedsTheMaximumLineLengthOf120CharactersAndShouldTriggerADiagnosticWarning { get; set; }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0003")
                .WithSpan(8, 1, 8, 137)
                .WithArguments("136", "120");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}