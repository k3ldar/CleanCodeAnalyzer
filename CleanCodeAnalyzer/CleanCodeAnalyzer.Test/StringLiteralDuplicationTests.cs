using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using VerifyCS = CleanCodeAnalyzer.Test.CSharpAnalyzerVerifier<CleanCodeAnalyzer.CleanCodeAnalyzer>;

namespace CleanCodeAnalyzer.Test
{
    [TestClass]
    public class StringLiteralDuplicationTests
    {
        [TestMethod]
        public async Task TestUniqueLiterals_NoDiagnostic()
        {
            var test = @"
class C
{
    void M()
    {
        string a = ""First unique string"";
        string b = ""Second unique string"";
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestDuplicateLiterals_ReportsDiagnostic()
        {
            var test = @"
class C
{
    void M()
    {
        string connStr = ""Server=localhost;Database=MyDB"";
        // ... later
        string backup = ""Server=localhost;Database=MyDB"";
    }
}";
            var expected = VerifyCS.Diagnostic("CC0010")
                .WithSpan(8, 25, 8, 57)
                .WithArguments("Server=localhost;Database=MyDB", "2");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestShortLiterals_NoDiagnostic()
        {
            var test = @"
class C
{
    void M()
    {
        string x = ""OK"";
        string y = ""OK"";
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestConstantDeclaration_NoDiagnostic()
        {
            var test = @"
class C
{
    private const string ConnectionString = ""Server=localhost"";
    
    void M()
    {
        var conn = ConnectionString;
        var other = ConnectionString;
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestTripleDuplicate_ReportsSecondAndThird()
        {
            var test = @"
class C
{
    void M()
    {
        var msg = ""Validation failed"";
        var log1 = ""Validation failed"";
        var log2 = ""Validation failed"";
    }
}";
            var expected1 = VerifyCS.Diagnostic("CC0010")
                .WithSpan(7, 20, 7, 39)
                .WithArguments("Validation failed", "3");

            var expected2 = VerifyCS.Diagnostic("CC0010")
                .WithSpan(8, 20, 8, 39)
                .WithArguments("Validation failed", "3");

            await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2);
        }
    }
}