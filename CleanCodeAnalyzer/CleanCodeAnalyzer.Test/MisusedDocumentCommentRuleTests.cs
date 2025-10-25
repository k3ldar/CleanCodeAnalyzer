using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using VerifyCS = CleanCodeAnalyzer.Test.CSharpAnalyzerVerifier<CleanCodeAnalyzer.CleanCodeAnalyzer>;

namespace CleanCodeAnalyzer.Test
{
    [TestClass]
    public class MisusedDocumentCommentRuleTests
    {
        [TestMethod]
        public async Task TestRegularComment_NoDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        internal void Test()
        {
            // This is a regular comment
            int a = 1;
        }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestValidClassDocumentComment_NoDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    /// <summary>
    /// This is a valid documentation comment for a class
    /// </summary>
    class TestClass
    {
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestValidMethodDocumentComment_NoDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        /// <summary>
        /// This is a valid documentation comment for a method
        /// </summary>
        public void TestMethod()
        {
        }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestMisusedDocumentCommentInMethodBody_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        internal void DocumentCommentAsComment()
        {
            {|#0:/// this is not a valid documentation comment|}
            int a = 1;
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0006")
                .WithLocation(0);

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestMultipleMisusedDocumentComments_ReportsMultipleDiagnostics()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        internal void Test()
        {
            {|#0:/// First misused comment|}
            int a = 1;
            
            {|#1:/// Second misused comment|}
            int b = 2;
        }
    }
}";
            var expected1 = VerifyCS.Diagnostic("CC0006")
                .WithLocation(0);

            var expected2 = VerifyCS.Diagnostic("CC0006")
                .WithLocation(1);

            await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2);
        }

        [TestMethod]
        public async Task TestValidPropertyDocumentComment_NoDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        /// <summary>
        /// This is a valid documentation comment for a property
        /// </summary>
        public int TestProperty { get; set; }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestValidFieldDocumentComment_NoDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        /// <summary>
        /// This is a valid documentation comment for a field
        /// </summary>
        private int _testField;
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestMisusedDocumentCommentWithMultipleLines_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        internal void Test()
        {
            {|#0:/// This is a misused|}
            /// multiline comment
            int a = 1;
        }
    }
}";
            // Note: Each /// line is a separate trivia, so we may get multiple diagnostics
            var expected = VerifyCS.Diagnostic("CC0006")
                .WithLocation(0);

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestMixedCommentsInMethod_ReportsOnlyMisused()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        /// <summary>
        /// Valid method documentation
        /// </summary>
        internal void Test()
        {
            // This is a regular comment - OK
            int a = 1;
            
            {|#0:/// This is misused|}
            int b = 2;
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0006")
                .WithLocation(0);

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestValidConstructorDocumentComment_NoDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        /// <summary>
        /// This is a valid documentation comment for a constructor
        /// </summary>
        public TestClass()
        {
        }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestMisusedDocumentCommentInIfBlock_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        internal void Test(bool condition)
        {
            if (condition)
            {
                {|#0:/// Not valid here|}
                Console.WriteLine(""Test"");
            }
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0006")
                .WithLocation(0);

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestMisusedDocumentCommentInLoop_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        internal void Test()
        {
            for (int i = 0; i < 10; i++)
            {
                {|#0:/// Loop comment misused|}
                Console.WriteLine(i);
            }
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0006")
                .WithLocation(0);

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestValidEnumDocumentComment_NoDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    /// <summary>
    /// Valid enum documentation
    /// </summary>
    enum TestEnum
    {
        /// <summary>
        /// Valid enum member documentation
        /// </summary>
        Value1
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestValidDelegateDocumentComment_NoDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    /// <summary>
    /// Valid delegate documentation
    /// </summary>
    public delegate void TestDelegate();
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
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
        internal void Test()
        {
        }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestValidInterfaceDocumentComment_NoDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    /// <summary>
    /// Valid interface documentation
    /// </summary>
    interface ITestInterface
    {
        /// <summary>
        /// Valid method documentation in interface
        /// </summary>
        void TestMethod();
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestMisusedDocumentCommentAfterStatement_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        internal void Test()
        {
            int a = 1;
            {|#0:/// This comment comes after a statement|}
            a = 2;
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0006")
                .WithLocation(0);

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}