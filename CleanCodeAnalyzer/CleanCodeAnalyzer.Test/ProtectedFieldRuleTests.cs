using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using VerifyCS = CleanCodeAnalyzer.Test.CSharpAnalyzerVerifier<CleanCodeAnalyzer.CleanCodeAnalyzer>;

namespace CleanCodeAnalyzer.Test
{
    [TestClass]
    public class ProtectedFieldRuleTests
    {
        [TestMethod]
        public async Task TestPrivateField_NoDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        private int privateField;
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestPublicField_NoDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        public int publicField;
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestInternalField_NoDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        internal int internalField;
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestProtectedField_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        protected int protectedField;
    }
}";
            var expected = VerifyCS.Diagnostic("CC0007")
                .WithSpan(8, 23, 8, 37)
                .WithArguments("protectedField");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestMultipleProtectedFields_ReportsMultipleDiagnostics()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        protected int firstField;
        protected string secondField;
    }
}";
            var expected1 = VerifyCS.Diagnostic("CC0007")
                .WithSpan(8, 23, 8, 33)
                .WithArguments("firstField");

            var expected2 = VerifyCS.Diagnostic("CC0007")
                .WithSpan(9, 26, 9, 37)
                .WithArguments("secondField");

            await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2);
        }

        [TestMethod]
        public async Task TestProtectedFieldWithMultipleVariables_ReportsMultipleDiagnostics()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        protected int field1, field2, field3;
    }
}";
            var expected1 = VerifyCS.Diagnostic("CC0007")
                .WithSpan(8, 23, 8, 29)
                .WithArguments("field1");

            var expected2 = VerifyCS.Diagnostic("CC0007")
                .WithSpan(8, 31, 8, 37)
                .WithArguments("field2");

            var expected3 = VerifyCS.Diagnostic("CC0007")
                .WithSpan(8, 39, 8, 45)
                .WithArguments("field3");

            await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2, expected3);
        }

        [TestMethod]
        public async Task TestProtectedInternalField_NoDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        protected internal int protectedInternalField;
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestProtectedProperty_NoDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        protected int ProtectedProperty { get; set; }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestProtectedFieldInBaseClass_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class BaseClass
    {
        protected int baseField;
    }

    class DerivedClass : BaseClass
    {
        private int derivedField;
    }
}";
            var expected = VerifyCS.Diagnostic("CC0007")
                .WithSpan(8, 23, 8, 32)
                .WithArguments("baseField");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestMixedFieldModifiers_ReportsOnlyProtected()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        private int privateField;
        protected int protectedField;
        public int publicField;
        internal int internalField;
    }
}";
            var expected = VerifyCS.Diagnostic("CC0007")
                .WithSpan(9, 23, 9, 37)
                .WithArguments("protectedField");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestProtectedStaticField_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        protected static int staticProtectedField;
    }
}";
            var expected = VerifyCS.Diagnostic("CC0007")
                .WithSpan(8, 30, 8, 50)
                .WithArguments("staticProtectedField");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestProtectedReadonlyField_NoDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        protected readonly int readonlyProtectedField;
    }
}";
            // readonly protected fields should no longer report a diagnostic
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestProtectedConstField_NoDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        protected const int ConstField = 42;
    }
}";
            // const fields are implicitly static and don't have instance state issues
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestProtectedFieldWithInitializer_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
        protected int initializedField = 100;
    }
}";
            var expected = VerifyCS.Diagnostic("CC0007")
                .WithSpan(8, 23, 8, 39)
                .WithArguments("initializedField");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestMultipleClassesWithProtectedFields_ReportsMultipleDiagnostics()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class FirstClass
    {
        protected int firstClassField;
    }

    class SecondClass
    {
        protected string secondClassField;
    }
}";
            var expected1 = VerifyCS.Diagnostic("CC0007")
                .WithSpan(8, 23, 8, 38)
                .WithArguments("firstClassField");

            var expected2 = VerifyCS.Diagnostic("CC0007")
                .WithSpan(13, 26, 13, 42)
                .WithArguments("secondClassField");

            await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2);
        }

        [TestMethod]
        public async Task TestProtectedFieldInNestedClass_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class OuterClass
    {
        class InnerClass
        {
            protected int innerField;
        }
    }
}";
            var expected = VerifyCS.Diagnostic("CC0007")
                .WithSpan(10, 27, 10, 37)
                .WithArguments("innerField");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestProtectedFieldWithDifferentTypes_ReportsDiagnostics()
        {
            var test = @"
using System;
using System.Collections.Generic;

namespace TestNamespace
{
    class TestClass
    {
        protected string stringField;
        protected int intField;
        protected List<string> listField;
        protected DateTime dateField;
    }
}";
            var expected1 = VerifyCS.Diagnostic("CC0007")
                .WithSpan(9, 26, 9, 37)
                .WithArguments("stringField");

            var expected2 = VerifyCS.Diagnostic("CC0007")
                .WithSpan(10, 23, 10, 31)
                .WithArguments("intField");

            var expected3 = VerifyCS.Diagnostic("CC0007")
                .WithSpan(11, 32, 11, 41)
                .WithArguments("listField");

            var expected4 = VerifyCS.Diagnostic("CC0007")
                .WithSpan(12, 28, 12, 37)
                .WithArguments("dateField");

            await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2, expected3, expected4);
        }

        [TestMethod]
        public async Task TestEmptyClass_NoDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    class TestClass
    {
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestProtectedFieldInAbstractClass_ReportsDiagnostic()
        {
            var test = @"
using System;

namespace TestNamespace
{
    abstract class AbstractClass
    {
        protected int abstractClassField;
    }
}";
            var expected = VerifyCS.Diagnostic("CC0007")
                .WithSpan(8, 23, 8, 41)
                .WithArguments("abstractClassField");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}